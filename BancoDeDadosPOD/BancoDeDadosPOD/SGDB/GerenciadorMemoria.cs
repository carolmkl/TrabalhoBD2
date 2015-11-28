using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BancoDeDadosPOD.SGDB
{

    public class GerenciadorMemoria
    {
        // Caminho para o diretório onde ficam todos os dados do banco de dados
        private string dirBanco;

        // Caminho para uma das subpastas do banco, os database
        private string dirBaseDados;

        // Metadados da Database selecionada
        private Dictionary<string, Metadados> metadados; //nome tabela, metadados da tabela

        // Instancia unica
        private static GerenciadorMemoria singleton;

        #region *** Construtores ***
        private GerenciadorMemoria()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod");
            }
            setDiretorioBanco(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod");
        }

        private GerenciadorMemoria(string diretorioPath)
        {
            setDiretorioBanco(diretorioPath);
        }

        private GerenciadorMemoria(string diretorioPath, string subPastaPath)
        {
            setDiretorioBanco(diretorioPath);
            setDatabase(subPastaPath);
        }

        public static GerenciadorMemoria getInstance()
        {
            if (singleton == null)
            {
                singleton = new GerenciadorMemoria();
            }
            return singleton;
        }
        #endregion

        #region *** Setters e Getters ***
        public void setDiretorioBanco(string diretorioBanco)
        {
            this.dirBanco = diretorioBanco;
        }

        public string getDiretorioBanco()
        {
            return dirBanco;
        }

        public void setDatabase(string subPastaPath)
        {
            if (subPastaPath != null && !Directory.Exists(dirBanco + "\\" + subPastaPath))
            {
                throw new SGDBException("Datadase " + subPastaPath + " não existe");
            }
            this.dirBaseDados = subPastaPath;
            metadados = recuperarMetadados();
        }

        public string getDatabase()
        {
            return dirBaseDados;
        }

        public string getPath()
        {
            return dirBanco + "\\" + dirBaseDados;
        }
        #endregion

        // Criar a base de dados e apontar como database atual
        public void createDatabase(string name)
        {
            if (Directory.Exists(dirBanco + "\\" + name))
            {
                throw new SGDBException("Database já existe");
            }

            Directory.CreateDirectory(dirBanco + "\\" + name);
            setDatabase(name);
        }

        private void criarArquivo(string nome, string extencao)
        {
            if (!File.Exists(dirBanco + "\\" + dirBaseDados + "\\" + nome + extencao))
            {
                using (File.Create(dirBanco + "\\" + dirBaseDados + "\\" + nome + extencao)) { }
            }
        }

        // Cria a tabela para não ter que criar durante a inserção
        private void createTable(string nome)
        {
            criarArquivo(nome, ".dat");
        }

        // Cria o indice para não ter que criar durante a inserção
        public void createIndex(string nome)
        {
            criarArquivo(nome, ".idx");
        }

        public bool dropIndex(string nome)
        {
            Metadados m;
            foreach (KeyValuePair<string, Metadados> item in metadados)
            {
                m = item.Value;
                if (m.getIndexes().ContainsKey(nome))
                {
                    m.getIndexes().Remove(nome);
                    File.Delete(dirBanco + "\\" + dirBaseDados + "\\" + nome + ".idx");
                    return true;
                }
            }

            return false;
        }

        private bool permitirDropTable(string nome)
        {
            Metadados meta = singleton.recuperarMetadados(nome);
            bool pode = true;
            for (int i = 0; i < meta.getNomesColunas().Count && pode; i++)
            {
                if (meta.getDados()[meta.getNomesColunas()[i]].isRForeing())
                {
                    pode = false;
                }
            }

            return pode;
        }

        public void dropTable(string nome)
        {
            if (permitirDropTable(nome))
            {
                Metadados metaExcluir, metaAux;
                metaExcluir = recuperarMetadados(nome);
                foreach (KeyValuePair<string, DadosTabela> dt in metaExcluir.getDados())
                {
                    if (dt.Value.isForeing())
                    {
                        metaAux = recuperarMetadados(dt.Value.getForeing()[0]);
                        metaAux.getDados()[dt.Value.getForeing()[1]].decForeing();
                        salvarMetadados(metaAux);
                    }
                }

                foreach (KeyValuePair<string, string[]> indices in metaExcluir.getIndexes())
                {
                    File.Delete(dirBanco + "\\" + dirBaseDados + "\\" + indices.Key + ".idx");
                }

                File.Delete(dirBanco + "\\" + dirBaseDados + "\\" + nome + ".meta");
                File.Delete(dirBanco + "\\" + dirBaseDados + "\\" + nome + ".dat");
            }
            else
            {
                throw new SGDBException("Tabela contém campos usados pra foreing keys");
            }
        }

        private void validarDataBase()
        {
            if (dirBaseDados == null)
            {
                throw new SGDBException("Database não selecionado");
            }
        }

        public bool existeTabela(string nome)
        {
            // se ainda ta na pasta principal;
            validarDataBase();

            // tentar verificar se existe a tabela
            try
            {
                return File.Exists(dirBanco + "\\" + dirBaseDados + "\\" + nome + ".meta");
            }
            catch
            {
                throw new SGDBException("Problemas ao executar a consulta");
            }
        }

        public bool existeIndice(string nome)
        {
            // se ainda ta na pasta principal;
            validarDataBase();

            // tentar verificar se existe o arquivo de indice
            Metadados m;
            foreach (KeyValuePair<string, Metadados> item in metadados)
            {
                m = item.Value;
                if (m.getIndexes().ContainsKey(nome))
                {
                    return true;
                }
            }

            return false;
        }

        // Esses dois tão feitos, se funcionam é outra história
        /// <summary>
        /// Utilizado quando cria uma tabela.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public bool salvarMetadados(Metadados meta)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dirBanco + "\\" + dirBaseDados + "\\" + meta.getNome() + ".meta", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, meta);
                stream.Close();
                createTable(meta.getNome());
                metadados.Add(meta.getNome(), meta);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void salvarMetadados()
        {
            foreach (KeyValuePair<string, Metadados> item in metadados)
            {
                salvarMetadados(item.Value);
            }
        }

        // ja cria a tabela pra não ter que criar durante a inserção
        private void criarTabela(string nome)
        {
            criarFile(nome, ".dat");
        }

        public void criarIndex(string nome)
        {
            criarFile(nome, ".idx");
        }

        private void criarFile(string nome, string extencao)
        {
            if (!File.Exists(dirBanco + "\\" + dirBaseDados + "\\" + nome + extencao))
            {
                File.Create(dirBanco + "\\" + dirBaseDados + "\\" + nome + extencao);
            }
        }

        public Metadados recuperarMetadados(string nome)
        {
            if (!metadados.ContainsKey(nome))
                throw new SGDBException("Tabela não existe");

            return metadados[nome];
        }

        public Dictionary<string, Metadados> recuperarMetadados()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                string[] arquivos = Directory.GetFiles(dirBanco + "\\" + dirBaseDados + "\\");
                Dictionary<string, Metadados> dados = new Dictionary<string, Metadados>();
                foreach (string arquivo in arquivos)
                {
                    if (arquivo.EndsWith(".meta"))
                    {
                        Stream stream = new FileStream(arquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Metadados meta = (Metadados)formatter.Deserialize(stream);
                        string tabela = new FileInfo(arquivo).Name;
                        tabela = tabela.Remove(tabela.IndexOf('.'));
                        dados.Add(tabela, meta);
                        stream.Close();
                    }
                }

                return dados;
            }
            catch
            {
                throw new SGDBException("Tabela não existe");
            }
        }

        public void atualizar()
        {
            metadados = recuperarMetadados();
        }
    }
}
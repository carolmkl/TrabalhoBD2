using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BancoDeDadosPOD.SGDB
{

    public class GerenciadorMemoria
    {
        // caminho para o diretório onde fica todos os dados do banco de dados
        private string diretorioPath;
        // caminho pra uma das subpastas do banco, os database
        private string pastaDatabase;
        // Metadados da Database selecionada
        private Dictionary<string, Metadados> metadados; //nome tabela, metadados da tabela

        private static GerenciadorMemoria singleton;
        // Construtores

        private GerenciadorMemoria()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod");
            }
            setDiretorioPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ bdPod");
        }

        private GerenciadorMemoria(string diretorioPath)
        { setDiretorioPath(diretorioPath); }

        private GerenciadorMemoria(string diretorioPath, string subPastaPath)
        {
            setDiretorioPath(diretorioPath);
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

        // Métodos

        public void setDiretorioPath(string diretorioPath)
        {
            this.diretorioPath = diretorioPath;
        }

        public string getDiretorioPath()
        {
            return diretorioPath;
        }

        public void setDatabase(string subPastaPath)
        {
            if (subPastaPath != null && !Directory.Exists(diretorioPath + "\\" + subPastaPath))
            {
                throw new SGDBException("Datadase " + subPastaPath + " não existe");
            }
            this.pastaDatabase = subPastaPath;
            metadados = recuperarMetadados();
        }

        public string getDatabase()
        {
            return pastaDatabase;
        }

        public string getPath()
        {
            return diretorioPath + "\\" + pastaDatabase;
        }

        // Cria a base de dados já set a coloca como database atual
        public void createDatabase(string name)
        {
            if (Directory.Exists(diretorioPath + "\\" + name))
            {
                throw new SGDBException("Database já existe");
            }
            Directory.CreateDirectory(diretorioPath + "\\" + name);
            setDatabase(name);
        }

        public bool existeTabela(string nome)
        {
            // se ainda ta na pasta principal;
            if (pastaDatabase == null)
            {
                throw new SGDBException("Database não selecionado");
            }

            // tentar verificar se existe a tabela
            try
            {
                return File.Exists(diretorioPath + "\\" + pastaDatabase + "\\" + nome + ".meta");
            }
            catch
            {
                throw new SGDBException("Problemas ao executar a consulta");
            }
        }

        public bool existeIndex(string nome)
        {
            // se ainda ta na pasta principal;
            if (pastaDatabase == null)
            {
                throw new SGDBException("Database não selecionado");
            }

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

        public bool excluirIndex(string nome)
        {
            Metadados m;
            foreach (KeyValuePair<string, Metadados> item in metadados)
            {
                m = item.Value;
                if (m.getIndexes().ContainsKey(nome))
                {
                    m.getIndexes().Remove(nome);
                    File.Delete(diretorioPath + "\\" + pastaDatabase + "\\" + nome + ".idx");
                    return true;
                }
            }
            return false;
        }

        private bool podeExcluirTable(string nome)
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

        public void excluirTable(string nome)
        {
            if (podeExcluirTable(nome))
            {
                Metadados metaExcluir, metaAux;
                metaExcluir = recuperarMetadados(nome);
                foreach (KeyValuePair<string, DadosTabela> dt in metaExcluir.getDados())
                {
                    if (dt.Value.isForeing())
                    {
                        metaAux = recuperarMetadados(dt.Value.getForeing()[0]);
                        metaAux.getDados()[dt.Value.getForeing()[1]].minusForeing();
                        salvarMetadados(metaAux);
                    }
                }


                File.Delete(diretorioPath + "\\" + pastaDatabase + "\\" + nome + ".meta");
                File.Delete(diretorioPath + "\\" + pastaDatabase + "\\" + nome + ".dat");
            }
            else
            {
                throw new SGDBException("Tabela contém campos usados pra foreing keys");
            }

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
                Stream stream = new FileStream(diretorioPath + "\\" + pastaDatabase + "\\" + meta.getNome() + ".meta", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, meta);
                stream.Close();
                criarTabela(meta.getNome());
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
            if (!File.Exists(diretorioPath + "\\" + pastaDatabase + "\\" + nome + extencao))
            {
                using (File.Create(diretorioPath + "\\" + pastaDatabase + "\\" + nome + extencao)) { }
            }
        }

        public Metadados recuperarMetadados(string nome)
        {
            if (!metadados.ContainsKey(nome)) throw new SGDBException("Tabela não existe");
            return metadados[nome];
        }

        public Dictionary<string, Metadados> recuperarMetadados()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                string[] arquivos = Directory.GetFiles(diretorioPath + "\\" + pastaDatabase + "\\");
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

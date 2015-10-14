using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB
{

    public class GerenciadorMemoria
    {
        // caminho para o diretório onde fica todos os dados do banco de dados
        private string diretorioPath;

        // caminho pra uma das subpastas do banco, os database
        private string subPastaPath;

        private static GerenciadorMemoria singleton;

        // Construtores

        private GerenciadorMemoria()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +"\\ bdPod"))
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
            setSubPastaPath(subPastaPath);
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

        public void setSubPastaPath(string subPastaPath)
        {
            if (subPastaPath != null && !Directory.Exists(diretorioPath+"\\"+subPastaPath))
            {
                throw new SGDBException("Datadase " + subPastaPath + " não existe");
            }
            this.subPastaPath = subPastaPath;
        }

        public string getSubPastaPath()
        {
            return subPastaPath;
        }

        // Cria a base de dados já set a coloca como database atual
        public void createDatabase(string name)
        {
            if (Directory.Exists(diretorioPath+"\\"+name))
            {
                throw new SGDBException("Database já existe");
            }
            Directory.CreateDirectory(diretorioPath + "\\" + name);
            subPastaPath = name;
        }

        public bool existeTabela(string nome)
        {
            // se ainda ta na pasta principal;
            if (subPastaPath == null)
            {
                throw new SGDBException("Database não selecionado");
            }

            // tentar verificar se existe a tabela
            try
            {
                return File.Exists(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".meta");
            }
            catch
            {
                throw new SGDBException("Problemas ao executar a consulta");
            }
        }

        public bool excluirIndex(string nome)
        {
            return false;
        }

        private bool podeExcluirTable(string nome)
        {
            Metadados meta = singleton.recuperarMetadados(nome);
            bool pode = true;
            for(int i=0; i<meta.getNomesColunas().Count && pode; i++)
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


                File.Delete(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".meta");
                File.Delete(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".dat");
            }
            else
            {
                throw new SGDBException("Tabela contém campos usados pra foreing keys");
            }

        }

        // Esses dois tão feitos, se funcionam é outra história
        public bool salvarMetadados(Metadados meta)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(diretorioPath + "\\" + subPastaPath + "\\"+ meta.getNome()+".meta", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, meta);
                stream.Close();
                criarTabela(meta.getNome());
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ja cria a tabela pra não ter que criar durante a inserção
        private void criarTabela(string nome)
        {
            if (!File.Exists(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".dat"))
            {
                File.Create(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".dat");
            }
        }

        public Metadados recuperarMetadados(string nome)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(diretorioPath + "\\" + subPastaPath + "\\"+nome +".meta", FileMode.Open, FileAccess.Read, FileShare.Read);
                Metadados meta = (Metadados)formatter.Deserialize(stream);
                stream.Close();
                return meta;
            }
            catch
            {
                throw new SGDBException("Tabela não existe");
            }
        }

    }
}

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

        // Construtores

        public GerenciadorMemoria()
        {}

        public GerenciadorMemoria(string diretorioPath)
        { setDiretorioPath(diretorioPath); }

        public GerenciadorMemoria(string diretorioPath, string subPastaPath)
        {
            setDiretorioPath(diretorioPath);
            setSubPastaPath(subPastaPath);
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
            if (!Directory.Exists(diretorioPath+"\\"+subPastaPath))
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
                if (File.Exists(diretorioPath + "\\" + subPastaPath + "\\" + nome + ".tab"))
                { return true;}
                else
                { return false;}
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

        public bool excluirTable(string nome)
        {
            // proteção de foreing key
            return false;
        }

        // Esses dois tão feitos, se funcionam é outra história
        public bool salvarMetadados(Metadados meta)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(meta.getNome()+".meta", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, meta);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Metadados recuperarMetadados(string nome)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(nome+".meta", FileMode.Open, FileAccess.Read, FileShare.Read);
                Metadados meta = (Metadados)formatter.Deserialize(stream);
                stream.Close();
                return meta;
            }
            catch
            {
                throw new SGDBException("Banco Corrompido!\nSalve tudo em um backup(não implementado),\nreintale o banco e recupere-o(não implementado)");
            }
        }

    }
}

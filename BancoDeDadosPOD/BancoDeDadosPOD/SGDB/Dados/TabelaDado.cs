using System.Collections.Generic;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public class TabelaDado
    {
        private string nome;
        private string path;
        private List<Registro> registros;

        public TabelaDado(string nome, string path)
        {
            this.nome = nome;
            this.path = path;
            registros = new List<Registro>();
        }

        internal List<Registro> Registros
        {
            get
            {
                return registros;
            }

            set
            {
                registros = value;
            }
        }

        public override string ToString()
        {
            string retorno = "";
            foreach (Registro registro in registros)
            {
                foreach (Dado item in registro.Dados)
                {
                    retorno += item.nome + " = " + item.valor + " " + item.isValido + " \n";
                }
            }
            

            return retorno;
        }
    }

    // classes sealed nao podem ser herdadas
    public sealed class Registro
    {
        private long posicao;
        private List<Dado> dados;

        public List<Dado> Dados
        {
            get
            {
                return dados;
            }

            set
            {
                dados = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id -1 vai pro final do arquivo(inserção)</param>
        public Registro(long id)
        {
            this.posicao = id;
            Dados = new List<Dado>();
        }

        public long getPosicao()
        {
            return posicao;
        }
    }

    // classes sealed nao podem ser herdadas
    public sealed class Dado
    {
        public string nome;
        private byte tamanho;
        public long posicao;
        public bool isValido { get; internal set; }
        public dynamic valor { get; internal set; }


        public Dado(string nome, byte tamanho, long posicao, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tamanho = tamanho;
            this.posicao = posicao;
            this.isValido = isValido;
            this.valor = valor;
        }

        public Dado(string nome, byte tamanho, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tamanho = tamanho;
            this.isValido = isValido;
            this.valor = valor;
        }

        public void setValor(bool isValido, dynamic valor = null)
        {
            this.isValido = isValido;
            this.isValido = valor;
        }

        public void setNulo()
        {
            this.isValido = false;
        }

        public string getValorStr()
        {
            return this.valor;
        }

        public int getValorInt()
        {
            return this.valor;
        }
    }
}
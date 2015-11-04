using System.Collections.Generic;

namespace BancoDeDadosPOD.SGDB
{
    class TabelaDado
    {
        private string nome;
        private string path;
        private List<Registro> registros;

        public TabelaDado(string nome, string path)
        {
            this.nome = nome;
            this.path = path;
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
    }

    internal sealed class Registro
    {
        private long posicao;
        public List<Dado> dados;

        public Registro(long id)
        {
            this.posicao = id;
            dados = new List<Dado>();
        }

        public long getPosicao()
        {
            return posicao;
        }
    }

    internal sealed class Dado
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

        public void setValor(bool isValido, dynamic valor = null)
        {
            this.isValido = isValido;
            this.isValido = valor;
        }

        public void setNulo()
        {
            this.isValido = false;
        }
    }
}

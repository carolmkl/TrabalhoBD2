using System;
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
        private long posicao;     // Byte de posicao me que o registro inicia dentro do arquivo
        private List<Dado> dados; // Lista de dados, cada dado uma coluna da tabela

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
        public Registro(long posicao)
        {
            this.posicao = posicao;
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
        public string nome { get; internal set; }   // Nome da coluna
        public TipoDado tipo { get; internal set; } // Tipo primitivo do dado
        public byte tamanho { get; internal set; }  // Tamanho em bytes de cada dado
        public long posicao { get; internal set; }  // Posicao do dado dentro do registro 0 a N
        public bool isValido { get; internal set; } // Define se o dado é nulo ou valido
        public dynamic valor { get; internal set; } // Valor que deve ser guardado

        public Dado(string nome, TipoDado tipo, byte tamanho, long posicao, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.tamanho = tamanho;
            this.posicao = posicao;
            this.isValido = isValido;
            this.valor = valor;
        }

        public Dado(string nome, TipoDado tipo, byte tamanho, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.tamanho = tamanho;
            this.posicao = -1;
            this.isValido = isValido;
            this.valor = valor;
        }

        public void setNulo()
        {
            this.isValido = false;
        }

        public void setValor(dynamic valor = null)
        {
            this.isValido = true;
            this.isValido = valor;
        }

        public string getValorStr()
        {
            return this.valor;
        }

        public int getValorInt()
        {
            return Convert.ToInt32(getValorStr().Replace('\"', ' ').Trim());
        }
    }

    public sealed class DadoIndice
    {
        public TipoDado tipo { get; }
        public dynamic valor { get; }

        public DadoIndice(TipoDado tipo, dynamic valor)
        {
            this.tipo = tipo;
            this.valor = valor;
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

    public enum TipoDado {
        Inteiro,
        String
    }
}
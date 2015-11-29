using System;
using System.Collections.Generic;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public enum TipoDado
    {
        Inteiro,
        String
    }

    // Classe responsável pelos dados de entrada e saida no arquivo de dados.
    public class TabelaDado
    {
        public string nome { get; set; }
        private string path;
        public List<RegistroTabela> registros { get; set; }

        public TabelaDado(string nome, string path)
        {
            this.nome = nome;
            this.path = path;
            registros = new List<RegistroTabela>();
        }

        public override string ToString()
        {
            string retorno = "";
            foreach (RegistroTabela registro in registros)
            {
                foreach (DadoTabela item in registro.dados)
                {
                    retorno += item.nome + " = " + item.valor + " " + item.isValido + " \n";
                }
            }

            return retorno;
        }
    }

    // Classe responsável por cada registro dentro da tabela
    public sealed class RegistroTabela
    {
        private long posicao;     // Byte de posicao me que o registro inicia dentro do arquivo
        public List<DadoTabela> dados; // Lista de dados, cada dado uma coluna da tabela

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id -1 vai pro final do arquivo(inserção)</param>
        public RegistroTabela(long posicao)
        {
            this.posicao = posicao;
            dados = new List<DadoTabela>();
        }

        public long getPosicao()
        {
            return posicao;
        }
    }

    // Classe responsável por cada dado dentro de cada registro dentro da tabela.
    public sealed class DadoTabela
    {
        public string nome { get; internal set; }   // Nome da coluna
        public TipoDado tipo { get; internal set; } // Tipo primitivo do dado
        public byte tamanho { get; internal set; }  // Tamanho em bytes de cada dado
        public long posicao { get; internal set; }  // Posicao do dado dentro do registro 0 a N
        public bool isValido { get; internal set; } // Define se o dado é nulo ou valido
        public dynamic valor { get; internal set; } // Valor que deve ser guardado

        #region *** Construtores ***
        public DadoTabela(string nome, TipoDado tipo, byte tamanho, long posicao, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.tamanho = tamanho;
            this.posicao = posicao;
            this.isValido = isValido;
            this.valor = valor;
        }

        public DadoTabela(string nome, TipoDado tipo, byte tamanho, bool isValido, dynamic valor)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.tamanho = tamanho;
            this.posicao = -1;
            this.isValido = isValido;
            this.valor = valor;
        }
        #endregion

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
            return Convert.ToInt32(this.valor);
        }
    }

    // Classe responsável por cada registro do indice.
    public sealed class RegistroIndice
    {
        public long posicao { get; internal set; }  // Posicao do byte inicial do registro na tabela
        public List<DadoIndice> dados;              // Dados que referenciam este registro do indice.

        public RegistroIndice()
        {
            this.posicao = posicao;
            this.dados = new List<DadoIndice>();
        }
    }

    // Classe responsável por cada dado de cada registro dentro do indice.
    public sealed class DadoIndice
    {
        public long posicao { get; }                // Posição ordinal do campo no registro do dado.
        public TipoDado tipo { get; internal set; } // Tipo primitivo do dado
        public dynamic valor { get; }               // Valor registrado no indice.

        #region *** Constutor ***
        public DadoIndice(TipoDado tipo, dynamic valor)
        {
            this.tipo = tipo;
            this.valor = valor;
        }
        #endregion

        public string getValorStr()
        {
            return this.valor;
        }

        public int getValorInt()
        {
            return Convert.ToInt32(this.valor);
        }
    }
}
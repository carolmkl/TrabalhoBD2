﻿using System.Collections.Generic;

namespace BancoDeDadosPOD.SGDB.Dados
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

    // classes sealed nao podem serem herdadas
    public sealed class Registro
    {
        private long posicao;
        public List<Dado> dados;

        // posicao -1 vai pro final do arquivo(inserção)
        public Registro(long posicao)
        {
            this.posicao = posicao;
            dados = new List<Dado>();
        }

        public long getPosicao()
        {
            return posicao;
        }
    }

    // classes sealed nao podem serem herdadas
    public sealed class Dado
    {
        public string nome { get; internal set; }
        public TipoDado tipo { get; internal set; }
        public byte tamanho { get; internal set; }
        public long posicao { get; internal set; }
        public bool isValido { get; internal set; }
        public dynamic valor { get; internal set; }


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
            return this.valor;
        }
    }

    public enum TipoDado {
        Inteiro,
        String
    }
}
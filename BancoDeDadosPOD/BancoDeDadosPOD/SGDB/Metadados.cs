using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    class DadosTablea
    {
        string nomeCampo;
        string tipo;
        int tamanho;
        bool primary;
        string[] foreing;

        DadosTablea()
        {
            foreing = new string[2];
        }

        DadosTablea(string nome, string tipo, int tamanho, bool primary, string[] foreing)
        {
            this.setNome(nome);
            this.setTipo(tipo);
            this.setTamanho(tamanho);
            this.primary = primary;
            this.foreing = foreing;
        }

        string getNomeCampo()
        {
            return nomeCampo;
        }

        void setNome(string nomeCampo)
        {
            this.nomeCampo = nomeCampo;
        }
        string geTipo()
        {
            return tipo;
        }

        void setTipo(string tipo)
        {
            this.tipo = tipo;
        }

        int getTamanho()
        {
            return tamanho;
        }

        void setTamanho(int tamanho)
        {
            this.tamanho = tamanho;
        }

        bool isPrimary()
        {
            return primary;
        }

        void setPrimary(bool primary)
        {
            this.primary = primary;
        }

        string[] getForeing()
        {
            return foreing;
        }

        void setForeing(string[] foreing)
        {
            this.foreing = foreing;
        }

        bool isForeing()
        {
            return this.foreing[0] != null;
        }
    }

    public class Metadados
    {
        private string nome;
        private List<DadosTablea> dados;

        public Metadados(String nome)
        {
            this.setNome(nome);
            dados = new List<DadosTablea>();
        }

        public string getNome()
        {
            return nome;
        }

        public void setNome(string nome)
        {
            this.nome = nome;
        }
    }
}

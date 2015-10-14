using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB
{
    [Serializable]
    public class DadosTabela
    {
        string nomeCampo;
        string tipo;
        int tamanho;
        bool primary;
        string[] foreing;

        public DadosTabela()
        {
            foreing = new string[2];
        }

        public DadosTabela(string nome, string tipo, int tamanho, bool primary, string[] foreing)
        {
            this.setNome(nome);
            this.setTipo(tipo);
            this.setTamanho(tamanho);
            this.primary = primary;
            this.foreing = foreing;
        }

        public DadosTabela(string nome, string tipo, int tamanho)
        {
            this.setNome(nome);
            this.setTipo(tipo);
            this.setTamanho(tamanho);
            this.primary = false;
            this.foreing = new string[2];
        }

        public string getNomeCampo()
        {
            return nomeCampo;
        }

        public void setNome(string nomeCampo)
        {
            this.nomeCampo = nomeCampo;
        }
        public string geTipo()
        {
            return tipo;
        }

        public void setTipo(string tipo)
        {
            this.tipo = tipo;
        }

        public int getTamanho()
        {
            return tamanho;
        }

        public void setTamanho(int tamanho)
        {
            this.tamanho = tamanho;
        }

        public bool isPrimary()
        {
            return primary;
        }

        public void setPrimary(bool primary)
        {
            this.primary = primary;
        }

        public string[] getForeing()
        {
            return foreing;
        }

        public void setForeing(string[] foreing)
        {
            this.foreing = foreing;
        }

        public void setForeing(string table, string colum)
        {
            foreing[0] = table;
            foreing[1] = colum;
        }

        public bool isForeing()
        {
            return String.IsNullOrEmpty(foreing[0]);
        }
    }

    [Serializable]
    public class Metadados
    {
        private string nome;
        //optei por dictonary pra facilitar a pesquisa
        private Dictionary<string, DadosTabela> dados;

        public Metadados()
        {
            dados = new Dictionary<string, DadosTabela>();
        }

        public Metadados(String nome)
        {
            this.setNome(nome);
            dados = new Dictionary<string, DadosTabela>();
        }

        public string getNome()
        {
            return nome;
        }

        public void setNome(string nome)
        {
            this.nome = nome;
        }

        public Dictionary<string, DadosTabela> getDados()
        {
            return dados;
        }

        public void addDados(DadosTabela dados)
        {
            this.dados[dados.getNomeCampo()] = dados;
        }

        public void addDados(string nome, string tipo, int tamanho, bool primary, string[] foreing)
        {
            dados[nome] = new DadosTabela(nome,tipo,tamanho,primary,foreing);
        }

        public void addDados(string nome, string tipo, int tamanho)
        {
            dados[nome] = new DadosTabela(nome, tipo, tamanho);
        }

        public string toString()
        {
            string descricao = nome + "\n";
            descricao += "Campo | Tipo | Tamanho | Primary |Foreing \n";
            foreach (KeyValuePair<string, DadosTabela> item in dados)
            {
                DadosTabela d = item.Value;
                descricao += d.getNomeCampo() +"|"+ d.geTipo() +"|"+ d.getTamanho() +"|"+ d.isPrimary() +"|"+ (d.isForeing()? d.getForeing()[0]+"(" +d.getForeing()[1]+")":"-") +"|"+ "\n";
            }
            return descricao;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoDeDadosPOD.SGDB
{
    [Serializable]
    public class DadosTabela
    {
        string nomeCampo;
        string tipo;
        int tamanho;
        bool primary;
        int contForeing;
        string[] foreing;

        public DadosTabela()
        {
            foreing = new string[2];
            this.contForeing = 0;
        }

        public DadosTabela(string nome, string tipo, int tamanho, bool primary, string[] foreing)
        {
            this.setNome(nome);
            this.setTipo(tipo);
            this.setTamanho(tamanho);
            this.primary = primary;
            this.foreing = foreing;
            this.contForeing = 0;
        }

        public DadosTabela(string nome, string tipo, int tamanho)
        {
            this.setNome(nome);
            this.setTipo(tipo);
            this.setTamanho(tamanho);
            this.primary = false;
            this.foreing = new string[2];
            this.contForeing = 0;
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
            return !String.IsNullOrEmpty(foreing[0]);
        }

        public bool isRForeing()
        {
            return contForeing>0;
        }

        public void addForeing()
        {
            contForeing++;
        }

        public void minusForeing()
        {
            contForeing--;
        }
    }

    [Serializable]
    public class Index
    {
        // nomes das colunas das tabelas
        private string[] nomesCampos;
        private Dictionary<string[], List<int>> indices;

        public Index(string[] nomesCampos)
        {
            indices = new Dictionary<string[], List<int>>();
            this.nomesCampos = nomesCampos;
        }

        public string[] getNomesCampos()
        {
            return nomesCampos;
        }

        public Dictionary<string[], List<int>> getIndices()
        {
            return indices;
        }

        public void addIndice(string[] chave, List<int> posi)
        {
            if (indices.ContainsKey(chave))
            {
                indices[chave].AddRange(posi);
                indices[chave].Sort();
            }
            else
            {
                posi.Sort();
                indices[chave] = posi;
            }
        }

        public void addIndice(string[] chave, int posi)
        {
            if (!indices.ContainsKey(chave))
            {
                indices[chave] = new List<int>();
            }
            indices[chave].Add(posi);
        }

    }

    [Serializable]
    public class Metadados
    {
        private string nome;
        //optei por dictonary pra facilitar a pesquisa
        private Dictionary<string, DadosTabela> dados;
        private List<string> nomesColunas;
        private Dictionary<string, Index> tabelaIndices;

        public Metadados()
        {
            dados = new Dictionary<string, DadosTabela>();
            nomesColunas = new List<string>();
            tabelaIndices = new Dictionary<string, Index>();
        }

        public Metadados(String nome)
        {
            this.setNome(nome);
            dados = new Dictionary<string, DadosTabela>();
            tabelaIndices = new Dictionary<string, Index>();
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
            addNomeColuna(dados.getNomeCampo());
            this.dados[dados.getNomeCampo()] = dados;
        }

        public void addDados(string nome, string tipo, int tamanho, bool primary, string[] foreing)
        {
            addNomeColuna(nome);
            dados[nome] = new DadosTabela(nome,tipo,tamanho,primary,foreing);
        }

        public void addDados(string nome, string tipo, int tamanho)
        {
            addNomeColuna(nome);
            dados[nome] = new DadosTabela(nome, tipo, tamanho);
        }

        private void addNomeColuna(string nome)
        {
            this.nomesColunas.Add(nome);
        }

        public List<string> getNomesColunas()
        {
            return nomesColunas;
        }

        public Dictionary<string, Index> getIndexes()
        {
            return tabelaIndices;
        }

        public void addIndice(TabelaSelect tabela, int lastPosi)
        {
            Index indice;
            List<string> dado;
            foreach (KeyValuePair<string, Index> item in tabelaIndices)
            {
                dado = new List<string>();
                indice = item.Value;
                for (int i = 0; i < indice.getNomesCampos().Count(); i++)
                {
                    dado.Add(tabela.Registros[0][nomesColunas.IndexOf(indice.getNomesCampos()[i])]);
                }
                indice.addIndice(dado.ToArray(), lastPosi);        
            }
        }

        public void criarIndiciePrimary()
        {
            List<string> campos = new List<string>();
            foreach (string item in nomesColunas)
            {
                if (dados[item].isPrimary())
                {
                    campos.Add(item);
                }
            }

            if(campos.Count != 0)
            {
                Index index = new Index(campos.ToArray());
                tabelaIndices["primary" + nome] = index;
            }

        }

        public void criarIndice(string nome, string[] campos)
        {
            Index index = new Index(campos);
            List<string> dado;
            TabelaSelect tabela = new TabelaSelect();
            int[] posis = null;
            int j = 0;
            // recuperar tudo com posicao
            dado = new List<string>();
            foreach (string[] item in tabela.Registros)
            {
                for (int i = 0; i < campos.Count(); i++)
                {
                    dado.Add(item[nomesColunas.IndexOf(campos[i])]);
                }
                index.addIndice(dado.ToArray(), posis[j]);
                j++;
            }
            tabelaIndices[nome] = index;
        }

        public override string ToString()
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

        public string StringIndices()
        {
            string desc = "";
            foreach (KeyValuePair<string, Index> item in tabelaIndices)
            {
                desc += item.Key + "\n\t";
                foreach (string i in item.Value.getNomesCampos())
                {
                    desc += i + "\t";
                }
                desc += "\n\t";
                foreach (KeyValuePair<string[], List<int>> j in item.Value.getIndices())
                {
                    foreach (string i in j.Key)
                    {
                        desc += i + "\t";
                    }
                    desc += "\n";
                    foreach (int i in j.Value)
                    {
                        desc += i + "\t";
                    }
                }
            }
            return desc;
        }
    }
}

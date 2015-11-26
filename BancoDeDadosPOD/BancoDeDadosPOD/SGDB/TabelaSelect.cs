using BancoDeDadosPOD.SGDB.Dados;
using BancoDeDadosPOD.SGDB.Select;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB
{
    public class TabelaSelect
    {
        string[] campos;
        List<string[]> registros;

        public TabelaSelect()
        {
            registros = new List<string[]>();
        }

        /// <summary>
        /// Ordena os registros conforme colunas passadas
        /// </summary>
        /// <param name="colunas">colunas a serem ordenadas por prioridade</param>
        /// <param name="asc">se a ordenação é ascendente</param>
        public void ordenaRegistros(List<string> colunas, bool asc)
        {
            for (int i = colunas.Count - 1; i >= 0; i--)
            {
                int pos;
                for (pos = 0; pos < campos.Length; pos++)
                {
                    if (campos[pos].Equals(colunas[i])) break;
                }
                if (asc)
                {
                    registros.Sort(delegate (string[] x, string[] y)
                    {
                        if (x[pos] == null && y[pos] == null) return 0;
                        else if (x[pos] == null) return -1;
                        else if (y[pos] == null) return 1;
                        else return x[pos].CompareTo(y[i]);
                    });
                }
                else
                {
                    registros.Sort(delegate (string[] x, string[] y)
                    {
                        if (x[i] == null && y[i] == null) return 0;
                        else if (x[i] == null) return 1;
                        else if (y[i] == null) return -1;
                        else return (x[i].CompareTo(y[i]) * -1);
                    });
                }
            }
        }

        /// <summary>
        /// Realiza a união da tabela passada com a tabela atual.
        /// Os campos devem ser iguais, senão lança exceção.
        /// </summary>
        /// <param name="outraTabela"></param>
        public void uniao(TabelaSelect outraTabela)
        {
            if (!igual(this.campos, outraTabela.Campos))
                throw new SGDBException("Para união de 2 tabelas, os campos devem ser iguais");
            this.registros.AddRange(outraTabela.registros);
        }

        /// <summary>
        /// Realiza a união da tabela passada com a tabela atual, depois realiza distinct com o resultado.
        /// Os campos devem ser iguais, senão lança exceção.
        /// </summary>
        /// <param name="outraTabela"></param>
        public void uniaoDistinct(TabelaSelect outraTabela)
        {
            if (!igual(this.campos, outraTabela.Campos))
                throw new SGDBException("Para união de 2 tabelas, os campos devem ser iguais");
            this.registros.AddRange(outraTabela.registros);
            registros = registros.Distinct().ToList();

        }



        /// <summary>
        /// realiza o INNER JOIN da tabela atual com a tabela passada conforme parametros da listaJoin
        /// e retorna o resultado do join
        /// </summary>
        /// <param name="outraTabela"></param>
        /// <param name="listaJoin"></param>
        /// <returns>resultado do Join</returns>
        public TabelaSelect join(TabelaSelect outraTabela, List<Filtro> listaJoin)
        {
            //para fazer o join será criado uma nova tabela com o resultado da operação
            TabelaSelect resultado = new TabelaSelect();
            int colunas = this.Campos.Length + outraTabela.Campos.Length;
            int iniDir = this.Campos.Length; //em que indice começam os registros da 2a tabela
            resultado.Campos = new string[colunas];
            //define os campos da nova tabela com a uniao das originais
            this.Campos.CopyTo(resultado.Campos, 0);
            outraTabela.Campos.CopyTo(resultado.Campos, iniDir);

            List<int[]> colFiltro = new List<int[]>();//lista contendo os indices em que serão verificados a igualdade
            
            //popula colFiltro
            foreach (Filtro filtro in listaJoin)
            {
                int colEsq = -1;
                int colDir = -1;
                //verifica se o filtro contempla as tabelas em questao
                if (resultado.Campos.Contains(filtro.RValue) && resultado.Campos.Contains(filtro.LValue))
                {
                    //define em que indice o valor será verificado na 1a tabela;
                    for (int i = 0; i < this.Campos.Length; i++)
                    {
                        if (this.Campos[i].Equals(filtro.RValue) || this.Campos[i].Equals(filtro.LValue))
                        {
                            colEsq = i;
                            break;
                        }
                    }
                    //define em que indice o valor será verificado na 2a tabela;
                    for (int i = 0; i < outraTabela.Campos.Length; i++)
                    {
                        if (outraTabela.Campos[i].Equals(filtro.RValue) || outraTabela.Campos[i].Equals(filtro.LValue))
                        {
                            colDir = i;
                            break;
                        }
                    }
                    //se os indices foram definidos corretamente compara registro a registro das 2 tabelas e insere no resultado se for igual
                    if (colEsq != -1 && colDir != -1)
                    {
                        colFiltro.Add(new int[] { colEsq, colDir });
                    }
                }
            }//foreach filtro

            //se nao tiver join retornar um produto cartesiano
            if (colFiltro.Count == 0)
            {
                foreach (string[] regEsq in this.Registros)
                {
                    foreach (string[] regDir in outraTabela.Registros)
                    {
                        string[] regTemp = new string[colunas];
                        regEsq.CopyTo(regTemp, 0);
                        regDir.CopyTo(regTemp, iniDir);
                        resultado.addRegistro(regTemp);
                    }
                }
            }
            else
            {
                foreach (string[] regEsq in this.Registros)
                {
                    foreach (string[] regDir in outraTabela.Registros)
                    {
                        bool insere = true;
                        foreach (int[] filtro in colFiltro)
                        {
                            if (!regEsq[filtro[0]].Equals(regDir[filtro[1]]))
                            {
                                insere = false;
                                break;
                            }
                        }
                        if (insere)
                        {
                            string[] regTemp = new string[colunas];
                            regEsq.CopyTo(regTemp, 0);
                            regDir.CopyTo(regTemp, iniDir);
                            resultado.addRegistro(regTemp);
                        }
                    }
                }

            }

            return resultado;
        }

        /// <summary>
        /// compara se as strings contidas nos parametros são iguais
        /// </summary>
        /// <param name="l1">valor a ser comparado com l2</param>
        /// <param name="l2">valor a ser comparado com l1</param>
        /// <returns></returns>
        private bool igual(string[] l1, string[] l2)
        {
            if (l1.Length != l2.Length) return false;
            for (int i = 0; i < l1.Length; i++)
            {
                if (!l1[i].Equals(l2[i])) return false;
            }
            return true;
        }

        public string[] Campos
        {
            get
            {
                return campos;
            }

            set
            {
                campos = value;
            }
        }

        public List<string[]> Registros
        {
            get
            {
                return registros;
            }
        }

        public void clearRegistros()
        {
            this.registros = new List<string[]>();
        }

        public void addRegistro(string[] reg)
        {
            if (reg.Length != campos.Length)
                throw new SGDBException("Registro adicionado à tabela não condiz com a quantidade de campos");
            registros.Add(reg);
        }

        /// <summary>
        /// Retorna uma TabelaSelect a partir de uma TabelaDado
        /// OBS.: Não traz os apelidos do select
        /// </summary>
        /// <param name="tabelaDado"></param>
        /// <returns></returns>
        public static TabelaSelect getTabelaSelect(TabelaDado tabelaDado)
        {
            if(tabelaDado == null || tabelaDado.Registros == null || tabelaDado.Registros.Count == 0)
            {
                return null;
            }
            TabelaSelect tabelaSelect = new TabelaSelect();
            List<Dado> dados = tabelaDado.Registros[0].Dados;
            int colunas = dados.Count;
            tabelaSelect.campos = new string[colunas];
            for (int i = 0; i < colunas; i++)
            {
                tabelaSelect.campos[i] = tabelaDado.Nome+"."+dados[i].nome;
            }
            foreach (Registro registro in tabelaDado.Registros)
            {
                dados = registro.Dados;
                string[] linha = new string[colunas];
                for (int i = 0; i < colunas; i++)
                {
                    linha[i] = dados[i].valor + "";
                }
                tabelaSelect.Registros.Add(linha);
            }
            return tabelaSelect;
        }

        public override string ToString()
        {
            string retorno = "| ";
            foreach (string item in campos)
            {
                retorno += item + " | ";
            }
            retorno += "\n";
            foreach (string[] item in registros)
            {
                retorno += "| ";
                foreach (string reg in item)
                {
                    retorno += reg + " | ";
                }
                retorno += "\n";
            }

            return retorno;
        }


    }
}

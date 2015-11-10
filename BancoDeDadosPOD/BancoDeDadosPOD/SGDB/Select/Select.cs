using BD2.Analizadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    class Select
    {
        static Select select;

        List<string> tabelas;
        Dictionary<string, string> retorno; //campo,apelido
        Where where;
        List<string> ordem;
        bool ordemAscendente;
        bool asterisco;

        public enum EtapaSemantica { CAMPOS, TABELA, JOIN, WHERE, ORDER }
        EtapaSemantica etapa = EtapaSemantica.CAMPOS;

        #region Construtor
        private Select()
        {
            tabelas = new List<string>();
            retorno = new Dictionary<string, string>();
            ordem = new List<string>();
            ordemAscendente = true;
            asterisco = false;
        }

        public static Select singleton()
        {
            if (select == null)
            {
                select = new Select();
            }
            return select;
        }
        #endregion

        /// <summary>
        /// Limpa todos os campos
        /// </summary>
        public void clear()
        {
            tabelas = new List<string>();
            retorno = new Dictionary<string, string>();
            Where = null;
            ordem = new List<string>();
            ordemAscendente = true;
            asterisco = false;
            Etapa = EtapaSemantica.CAMPOS;
        }

        /// <summary>
        /// adiciona um campo para ordenação crescente. valida a existencia deste.
        /// </summary>
        /// <param name="campo"></param>
        public void addOrderBy(string campo)
        {
            if (!retorno.Keys.Contains(campo)) throw new SemanticError("Campo " + campo + ", do ORDER BY, não consta como retorno.");
            ordem.Add(campo);
        }

        /// <summary>
        /// define a ordenação de todos os campos para decrescente
        /// </summary>
        public void orderDesc()
        {
            ordemAscendente = false;
        }

        /// <summary>
        /// Verifica se a tabela já foi inserida. Caso contrário, insere.
        /// </summary>
        /// <param name="tabela"></param>
        public void addTabela(string tabela)
        {
            if (!tabelas.Exists(c => c.Equals(tabela)))
            {
                tabelas.Add(tabela);
            }
        }

        /// <summary>
        /// Verifica se as tabelas solicitadas nos campos constam na cláusula from.
        /// Se alguma tabela não foi declarada lança um SemanticError
        /// A tabela pode ser declarada mas não utilizada, pois pode ser usada no Where
        /// </summary>
        /// <param name="tblVerifica"></param>
        public void verificaTabelas(List<string> tblVerifica)
        {
            foreach (string s in tabelas)
            {
                if (!tblVerifica.Exists(c => c.Equals(s))) throw new SemanticError("Tabela " + s + " não declarada na cláusula FROM");
            }
            foreach (string s in tblVerifica)
            {
                addTabela(s);
            }
        }

        /// <summary>
        /// Insere mais um campo de retorno. Não remove duplicados.
        /// Não trata o campo *
        /// </summary>
        /// <param name="retorno"></param>
        public void addRetorno(string retorno)
        {
            if (retorno.Equals("*")) throw new SGDBException("método Select.addRetorno não trata o *");
            if (this.retorno.Keys.Contains(retorno)) throw new SemanticError("Não é possível adicionar o mesmo campo 2x na consulta (" + retorno + ")");
            this.retorno.Add(retorno, retorno);
        }

        /// <summary>
        /// Insere o apelido passado no último retorno incluído
        /// </summary>
        /// <param name="apelido"></param>
        public void addApelidoUltimo(string apelido)
        {
            string key = retorno.ElementAt(retorno.Count - 1).Key;
            retorno[key] = apelido;
        }

        public TabelaSelect run()
        {
            TabelaSelect tabelaSelect = null;
            if (asterisco)
            {
                if (where != null)
                {
                    if (where.ListaFiltro == null || where.ListaFiltro.Count == 0)
                    {
                        //tabelaSelect = returnDados(tabelas[0]);
                    }
                    foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                    {
                        TabelaSelect tabela2 = null;
                        //tabela2 = returnDados(List < Filtro > filtro, string tabela)
                        if (tabelaSelect == null) tabelaSelect = tabela2;
                        else tabelaSelect.uniaoDistinct(tabela2);
                    }
                }
                else
                {
                    //tabelaSelect = returnDados(tabelas[0]);
                }
                if (ordem.Count > 0)
                {
                    tabelaSelect.ordena(ordem, ordemAscendente);
                }
                return tabelaSelect;
            }
            foreach (string s in tabelas)
            {
                //TODO: continuar
                //TODO: Incluir ordenação
            }

            return null;
        }


        #region Getter e Setter
        public Where Where
        {
            get
            {
                return where;
            }

            set
            {
                where = value;
            }
        }

        public EtapaSemantica Etapa
        {
            get
            {
                return etapa;
            }

            set
            {
                etapa = value;
            }
        }

        public bool Asterisco
        {
            get
            {
                return asterisco;
            }

            set
            {
                asterisco = value;
            }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            StringBuilder estrutura = new StringBuilder();
            estrutura.AppendLine("ESTRUTURA SELECT:");

            estrutura.Append("CAMPOS: ");
            foreach (string key in retorno.Keys)
            {
                estrutura.Append(key + " AS " + retorno[key] + ", ");
            }
            estrutura.Remove(estrutura.Length - 2, 2);
            estrutura.AppendLine();


            estrutura.Append("FROM: ");
            foreach (string t in tabelas)
            {
                estrutura.Append(t + ", ");
            }
            estrutura.Remove(estrutura.Length - 2, 2);
            estrutura.AppendLine();


            estrutura.Append("JOIN: ");
            if (where != null)
            {
                foreach (Filtro f in where.ListaJoin)
                {
                    estrutura.Append(f.LValue + " " + f.Op + " " + f.RValue + ", ");
                }
                estrutura.Remove(estrutura.Length - 2, 2);
            }
            else
            {
                estrutura.AppendLine("Não tem.");
            }
            estrutura.AppendLine();


            estrutura.Append("WHERE: ");
            if (where != null)
            {
                foreach (List<Filtro> lista in where.ListaFiltro)
                {
                    estrutura.Append("(");
                    foreach (Filtro f in lista)
                    {
                        estrutura.Append(f.LValue + " " + f.Op + " " + f.RValue + " AND ");
                    }
                    estrutura.Remove(estrutura.Length - 4, 4);
                    estrutura.Append(") OR ");
                    estrutura.AppendLine();
                }
                estrutura.Remove(estrutura.Length - 3, 3);
            }
            else
            {
                estrutura.AppendLine("Não tem.");
            }
            estrutura.AppendLine();


            estrutura.Append("ORDER BY: ");

            if (ordem.Count > 0)
            {
                foreach (string o in ordem)
                {
                    estrutura.Append(o + ", ");
                }
                estrutura.Remove(estrutura.Length - 2, 2);
                if (!ordemAscendente) estrutura.AppendLine(" DESC");
            }
            else
            {
                estrutura.AppendLine("Não tem.");
            }

            return estrutura.ToString();
        }
        #endregion

    }
}

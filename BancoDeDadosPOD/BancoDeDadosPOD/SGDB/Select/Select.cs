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

        List<Metadados> tabelas;
        Dictionary<string, string> retorno; //campo,apelido
        Where where;
        List<string> ordem;
        bool ordemAscendente;
        bool asterisco;
        GerenciadorMemoria mem = GerenciadorMemoria.getInstance();

        public enum EtapaSemantica { CAMPOS, TABELA, JOIN, WHERE, ORDER }
        EtapaSemantica etapa = EtapaSemantica.CAMPOS;

        #region Construtor
        private Select()
        {
            tabelas = new List<Metadados>();
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
            tabelas = new List<Metadados>();
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
            if (!mem.existeTabela(tabela))
            {
                throw new SemanticError("Tabela " + tabela + " não existe.");
            }
            if (!tabelas.Exists(c => c.Equals(tabela)))
            {
                tabelas.Add(mem.recuperarMetadados(tabela));
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
            foreach (Metadados m in tabelas)
            {
                if (!tblVerifica.Exists(c => c.Equals(m.getNome()))) throw new SemanticError("Tabela " + m.getNome() + " não declarada na cláusula FROM");
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

        /// <summary>
        /// método responsável por retornar o resultado do SELECT
        /// </summary>
        /// <returns>TabelaSelect formatada para apresentar no Form1</returns>
        public TabelaSelect run()
        {
             
            TabelaSelect tabelaSelect = null;
            //caso e select seja select tabela.* from tabela não será necessário 
            //aplicar join pois tratará de apenas uma tabela
            //tratamento apenas para where
            if (asterisco)
            {
                if (where != null)
                {
                    if (where.ListaFiltro == null || where.ListaFiltro.Count == 0)
                    {
                        //se não tiver filtro retorna tudo
                        tabelaSelect = GambiarraSelect.getInstance().returnDados(tabelas[0]);
                    }
                    //traz os resultados filtrados por grupos de AND e depois junta com os OR's
                    foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                    {
                        TabelaSelect tabelaFiltro = null;
                        tabelaFiltro = GambiarraSelect.getInstance().returnDados(filtrosAND, tabelas[0]);
                        if (tabelaSelect == null) tabelaSelect = tabelaFiltro;
                        else tabelaSelect.uniaoDistinct(tabelaFiltro);
                    }
                }
                else
                {
                    //se nao tiver filtro retorna tudo
                    tabelaSelect = GambiarraSelect.getInstance().returnDados(tabelas[0]);
                }
                //envia comando para a TabelaSelect ordenar os registros
                if (ordem.Count > 0)
                {
                    tabelaSelect.ordenaRegistros(ordem, ordemAscendente);
                }
                return tabelaSelect;
            }

            //Se não tem asterisco o negócio complica

            //ordena as tabelas por qtdade registros
            tabelas.Sort(delegate(Metadados m1,Metadados m2)
            {
                return m1.getNumeroRegistros() > m2.getNumeroRegistros() ? 1 : -1;
            });
            foreach (Metadados s in tabelas)
            {
                TabelaSelect tabelaFiltro = null;
                //filtra as colunas relacionadas com a tabela
                List<string> camposBuscar = retorno.Keys.Where(c => c.StartsWith(s.getNome())).ToList<string>();
                //O Join pode ter colunas que não constam como retorno, mas é necessário para juntar as tabelas depois
                //Adicionando campos de join para retorno.
                foreach (Filtro f in where.ListaJoin)
                {
                    if (f.LValue.StartsWith(s.getNome()) && !camposBuscar.Contains(f.LValue))
                        camposBuscar.Add(f.LValue);
                    if (f.RValue.StartsWith(s.getNome()) && !camposBuscar.Contains(f.RValue))
                        camposBuscar.Add(f.RValue);
                }
                //traz os resultados filtrados por grupos de AND e depois junta com os OR's
                foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                {
                    TabelaSelect tabelaFiltroOR = null;
                    //informa apenas os filtros relacionados com a tabela em questão
                    tabelaFiltroOR = GambiarraSelect.getInstance().returnDados(filtrosAND.Where(filtro => filtro.LValue.StartsWith(s.getNome())).ToList<Filtro>(), tabelas[0]);
                    if (tabelaFiltro == null) tabelaFiltro = tabelaFiltroOR;
                    else tabelaFiltro.uniaoDistinct(tabelaFiltroOR);
                }

                //se tem mais tabelas faz o join
                if (tabelaSelect == null) tabelaSelect = tabelaFiltro;
                else tabelaSelect = tabelaSelect.join(tabelaFiltro, Where.ListaJoin);
            }
            //envia comando para a TabelaSelect ordenar os registros
            if (ordem.Count > 0)
            {
                tabelaSelect.ordenaRegistros(ordem, ordemAscendente);
            }
            return tabelaSelect;
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

        public Dictionary<string, string> Retorno
        {
            get
            {
                return retorno;
            }

            set
            {
                retorno = value;
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
            foreach (Metadados t in tabelas)
            {
                estrutura.Append(t.getNome() + ", ");
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

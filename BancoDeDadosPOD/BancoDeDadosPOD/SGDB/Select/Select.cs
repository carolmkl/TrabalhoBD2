using BancoDeDadosPOD.SGDB.Dados;
using BD2.Analizadores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (!tabelas.Exists(c => c.getNome().Equals(tabela)))
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
        /// este método define qual o método será chamado para o select com where
        /// </summary>
        /// <param name="filtrosAND"></param>
        /// <param name="tabela"></param>
        /// <returns></returns>
        private TabelaSelect returnDados(List<Filtro> filtrosAND,Dictionary<string,List<string>> filtrosJoin, Metadados tabela)
        {
			// não funcionou corretamente após trazer gambiarra select para dentro.
            //return Base.getInstance().arqBinarios[tabela.getNome()].returnDados(filtrosAND, filtrosJoin, tabela);

            // manter este
            return GambiarraSelect.getInstance().returnDados(filtrosAND, filtrosJoin, tabela);
        }

        /// <summary>
        /// este método define qual o método será chamado para o select sem where
        /// </summary>
        /// <param name="tabela"></param>
        /// <returns></returns>
        private TabelaSelect returnDados(Metadados tabela)
        {
            // não funcionou corretamente após trazer gambiarra select para dentro.
            //return Base.getInstance().arqBinarios[tabela.getNome()].returnDados(tabela);

            // manter este
            return GambiarraSelect.getInstance().returnDados(tabela);
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
                string arqTabela = mem.getPath() + "\\" + tabelas[0].getNome() + ".dat";
                if (where != null)
                {
                    if (where.ListaFiltro == null || where.ListaFiltro.Count == 0)
                    {
                        //se não tiver filtro retorna tudo
                        //tabelaSelect = new Binarios(arqTabela).returnDados(tabelas[0]);
                        tabelaSelect = returnDados(tabelas[0]);
                    }
                    //traz os resultados filtrados por grupos de AND e depois junta com os OR's
                    foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                    {
                        TabelaSelect tabelaFiltro = null;
                        tabelaFiltro = returnDados(filtrosAND,new Dictionary<string, List<string>>(), tabelas[0]);
                        if (tabelaSelect == null) tabelaSelect = tabelaFiltro;
                        else tabelaSelect.uniaoDistinct(tabelaFiltro);
                    }
                }
                else
                {
                    //se nao tiver filtro retorna tudo

                    tabelaSelect = returnDados(tabelas[0]);
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
            tabelas.Sort(delegate (Metadados m1, Metadados m2)
            {
                return m1.getNumeroRegistrosTabela() > m2.getNumeroRegistrosTabela() ? -1 : 1;
            });


            Dictionary<Metadados, Where> filtros = new Dictionary<Metadados, Where>();
            //separa os filtros referentes a cada tabela e, se tiver filtro, joga a tabela no inicio da ordenação
            for (int i = 0; i < tabelas.Count; i++)
            {
                Metadados m = tabelas[i];
                Where filtroE = new Where();
                //separa os filtros
                foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                {
                    List<Filtro> maisFiltro = (filtrosAND.Where(f => f.LValue.StartsWith(m.getNome())).ToList());
                    filtroE.ListaFiltro.Add(maisFiltro);
                }
                //ordena pro inicio
                if (filtroE.ListaFiltro.Count > 0)
                {
                    m = tabelas[i];
                    tabelas.Remove(m);
                    tabelas.Insert(0, m);
                }
                filtros.Add(m, filtroE);
            }
            //seleciona cada tabela separadamente
            while (tabelas.Count > 0)
            {
                Metadados m = tabelas.First();
                TabelaSelect tabelaFiltro = null;
                //traz os resultados filtrados por grupos de AND e depois junta com os OR's
                if (filtros[m].ListaFiltro.Count > 0)
                {
                    foreach (List<Filtro> filtrosAND in where.ListaFiltro)
                    {
                        TabelaSelect tabelaFiltroOR = null;
                        //informa apenas os filtros relacionados com a tabela em questão
                        tabelaFiltroOR = returnDados(filtrosAND,filtros[m].FiltroJoin, m);
                        if (tabelaFiltro == null) tabelaFiltro = tabelaFiltroOR;
                        else tabelaFiltro.uniaoDistinct(tabelaFiltroOR);
                    }
                }
                else
                {
                    tabelaFiltro = returnDados(m);
                }

                tabelas.Remove(m);
                filtros.Remove(m);
                //Adicionando campos de join como filtro.
                
                foreach (Filtro f in where.ListaJoin)
                {
                    if (f.LValue.StartsWith(m.getNome()))
                    {
                        Metadados outroM = null;
                        foreach (Metadados meta in filtros.Keys)
                        {
                            if (f.RValue.StartsWith(meta.getNome()))
                            {
                                outroM = meta;
                                break;
                            }
                        }
                        //insere os registros de join como filtro para as proximas tabelas
                        if (outroM != null)
                        {
                            List<string> maisFiltro = new List<string>();
                            int colEsq = 0;
                            for (int i = 0; i < tabelaFiltro.Campos.Length; i++)
                            {
                                if (tabelaFiltro.Campos[i].Equals(f.LValue))
                                {
                                    colEsq = i;
                                    break;
                                }
                            }
                            foreach (string[] reg in tabelaFiltro.Registros)
                            {
                                maisFiltro.Add(reg[colEsq]);
                            }
                            maisFiltro.Sort();
                            filtros[outroM].FiltroJoin.Add(f.LValue, maisFiltro);

                            //joga o outroM para ser o proximo a pesquisar
                            tabelas.Remove(outroM);
                            tabelas.Insert(0, outroM);
                        }
                    }
                    if (f.RValue.StartsWith(m.getNome()))
                    {
                        Metadados outroM = null;
                        foreach (Metadados meta in filtros.Keys)
                        {
                            if (f.LValue.StartsWith(meta.getNome()))
                            {
                                outroM = meta;
                                break;
                            }
                        }
                        //insere os registros de join como filtro para as proximas tabelas
                        if (outroM != null)
                        {
                            List<string> maisFiltro = new List<string>();
                            int colDir = 0;
                            for (int i = 0; i < tabelaFiltro.Campos.Length; i++)
                            {
                                if (tabelaFiltro.Campos[i].Equals(f.LValue))
                                {
                                    colDir = i;
                                    break;
                                }
                            }
                            foreach (string[] reg in tabelaFiltro.Registros)
                            {
                                maisFiltro.Add(reg[colDir]);
                            }
                            maisFiltro.Sort();
                            filtros[outroM].FiltroJoin.Add(f.RValue, maisFiltro);

                            //joga o outroM para ser o proximo a pesquisar
                            tabelas.Remove(outroM);
                            tabelas.Insert(0, outroM);
                        }
                    }
                }/**/
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

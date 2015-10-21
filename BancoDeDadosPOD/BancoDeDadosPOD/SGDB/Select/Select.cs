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
        Dictionary<string,string> retorno; //campo,apelido
        Where filtro;
        Dictionary<string, bool> ordem;

        private Select()
        {
            tabelas = new List<string>();
            retorno = new Dictionary<string, string>();
            filtro = new Where();
            ordem = new Dictionary<string, bool>();
        }

        public static Select singleton()
        {
            if(select == null)
            {
                select = new Select();
            }
            return select;
        }

        /// <summary>
        /// Limpa todos os campos
        /// </summary>
        public void clear()
        {
            tabelas = new List<string>();
            retorno = new Dictionary<string, string>();
            filtro = new Where();
            ordem = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Verifica se a tabela já foi inserida. Caso contrário, insere.
        /// </summary>
        /// <param name="tabela"></param>
        public void addTabela(string tabela)
        {
            if (!tabelas.Contains(tabela))
            {
                tabelas.Add(tabela);
            }
        }

        /// <summary>
        /// Insere mais um campo de retorno. Não remove duplicados.
        /// </summary>
        /// <param name="retorno"></param>
        public void addRetorno(string retorno)
        {
            if (retorno.Equals("*"))
            {
                //TODO: retornar todas as colunas da tabela pelo metadados
            }
            this.retorno.Add(retorno,retorno);
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

    }
}

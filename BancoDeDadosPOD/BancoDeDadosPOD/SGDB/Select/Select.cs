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
        Dictionary<string,string> retorno; //campo,apelido
        Where filtro;
        Dictionary<string, bool> ordem;

        private Select()
        {
            tabelas = new List<string>();
            retorno = new Dictionary<string, string>();
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
            Filtro = null;
            ordem = new Dictionary<string, bool>();
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
                if (!tblVerifica.Exists(c => c.Equals(s))) throw new SemanticError("TabelaSelect " + s + " não declarada na cláusula FROM");
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

        public Where Filtro
        {
            get
            {
                return filtro;
            }

            set
            {
                filtro = value;
            }
        }

    }
}

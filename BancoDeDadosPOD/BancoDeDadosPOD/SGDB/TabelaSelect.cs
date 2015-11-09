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
        Dictionary<string, string> apelidos;
        List<string[]> registros;

        public TabelaSelect()
        {
            registros = new List<string[]>();
            apelidos = new Dictionary<string, string>();
        }

        /// <summary>
        /// Realiza a união da tabela passada com a tabela atual.
        /// Os campos devem ser iguais, senão lança exceção.
        /// </summary>
        /// <param name="outraTabela"></param>
        public void Uniao(TabelaSelect outraTabela)
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
        public void UniaoDistinct(TabelaSelect outraTabela)
        {
            if (!igual(this.campos, outraTabela.Campos))
                throw new SGDBException("Para união de 2 tabelas, os campos devem ser iguais");
            this.registros.AddRange(outraTabela.registros);
            registros = registros.Distinct().ToList();

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

        public Dictionary<string, string> Apelidos
        {
            get
            {
                return apelidos;
            }
        }

        public void clearApelidos()
        {
            apelidos = new Dictionary<string, string>();
        }

        public void addApelido(string campo, string apelido)
        {
            if (!campos.Contains(campo))
                throw new SGDBException("TabelaSelect não contém o campo '" + campo + "' para aplicar o apelido '" + apelido + "'");
            apelidos.Add(campo, apelido);
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

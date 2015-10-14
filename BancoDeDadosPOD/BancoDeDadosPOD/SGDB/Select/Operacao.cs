using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    class Operacao
    {
        private string lValue;
        private OperadorRel op;
        private string rValue;

        /// <summary>
        /// Valor utilizado do lado esquerdo da operação
        /// </summary>
        public string LValue
        {
            get
            {
                return lValue;
            }

            set
            {
                lValue = value;
            }
        }

        /// <summary>
        /// Valor utilizado do lado direito da operação. Pode ser um número, literal ou outro campo
        /// Validar:
        /// Caso seja outro campo, o Operador deve ser 'Igual'
        /// Caso seja literal, o Operador pode ser 'Igual' ou 'Diferente'
        /// </summary>
        public string RValue
        {
            get
            {
                return rValue;
            }

            set
            {
                rValue = value;
            }
        }

        internal OperadorRel Op
        {
            get
            {
                return op;
            }

            set
            {
                op = value;
            }
        }
    }
}

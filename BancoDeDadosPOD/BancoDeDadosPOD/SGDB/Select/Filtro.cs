﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    public class Filtro
    {
        private string lValue;
        private OperadorRel op;
        private string rValue;
        private bool isAND;
        private bool isOR;

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
                rValue = rValue.Replace('\'', ' ').Trim();
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

        /// <summary>
        /// necessário na hora de armazenar o filtro, mas nao na hora de ler
        /// </summary>
        public bool IsAND
        {
            get
            {
                return isAND;
            }

            set
            {
                isAND = value;
            }
        }

        /// <summary>
        /// necessário na hora de armazenar o filtro, mas nao na hora de ler
        /// </summary>
        public bool IsOR
        {
            get
            {
                return isOR;
            }

            set
            {
                isOR = value;
            }
        }
    }
}
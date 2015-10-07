using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    class ListaDeSimbolos
    {
        private static ListaDeSimbolos singleton;

        private ListaDeSimbolos() { }


        public static ListaDeSimbolos getInstance()
        {
            if (singleton == null)
            {
                singleton = new ListaDeSimbolos();
            }
            return singleton;
        }

        public String classeToken(int id)
        {
            switch (id)
            {
                case 2: return "identificador";

                case 3: return "constante numerica";

                case 4: return "constante literal";

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36: return "palavra reservada";

                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49: return "simbolo especial";

                default: return "token não reconhecido";
            }

        }
    }
}

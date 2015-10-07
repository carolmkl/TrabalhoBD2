using BancoDeDadosPOD.SGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    public class Semantico : Constants
    {
        public void executeAction(int action, Token token) 
        {
            switch (action)
            {
                case 0:
                    throw new SGDBException("Ação "+ action + " não implementada.");
                    break;
                case 1:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 2:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 3:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 4:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 5:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 6:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 7:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 8:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 9:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 10:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 11:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 12:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 13:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 14:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 15:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 16:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 17:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 18:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 19:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 20:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 21:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 22:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 23:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 24:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 25:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 26:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 28:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 29:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
            }
            Console.WriteLine("Ação #" + action + ", Token: " + token);
        }
    }
}

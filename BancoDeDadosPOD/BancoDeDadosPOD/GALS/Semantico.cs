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
            Console.WriteLine("Ação #" + action + ", Token: " + token);
        }
    }
}

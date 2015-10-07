using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB
{
    class SGDBException: Exception
    {
        public SGDBException (string msgm):base(msgm)
        { }

        public override string Message
        {
            get
            {
                return "Falha SGDB >> " + base.Message;
            }
        }
    }
}

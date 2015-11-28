using System;

namespace BancoDeDadosPOD.SGDB
{
    class SGDBException: Exception
    {
        private int position;

        public SGDBException(string msg, int position) : base(msg)
        {
            this.position = position;
        }

        public SGDBException (string msgm):base(msgm)
        { }

        public override string ToString()
        {
            return "Falha SGDB >> " + base.Message;
        }

        public override string Message
        {
            get
            {
                return "Falha SGDB >> " + base.Message;
            }
        }
    }
}
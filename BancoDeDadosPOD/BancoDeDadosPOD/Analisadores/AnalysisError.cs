using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    public class AnalysisError : Exception
    {
        private int position;

        public AnalysisError(string msg, int position) : base(msg)
        {
            this.position = position;
        }

        public AnalysisError(string msg) : base(msg)
        {    
            this.position = -1;
        }

        public int getPosition()
        {
            return position;
        }

        public override string ToString()
        {
            return base.Message + " na linha " + position;
        }
    }

}

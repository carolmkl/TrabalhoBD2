using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    class Where
    {
        private List<List<Operacao>> listaFiltro; //A lista mais externa contém os agrupamentos de OU
        private List<Operacao> listaJoin;

        internal List<List<Operacao>> ListaFiltro
        {
            get
            {
                return listaFiltro;
            }

            set
            {
                listaFiltro = value;
            }
        }

        internal List<Operacao> ListaJoin
        {
            get
            {
                return listaJoin;
            }

            set
            {
                listaJoin = value;
            }
        }
    }
}

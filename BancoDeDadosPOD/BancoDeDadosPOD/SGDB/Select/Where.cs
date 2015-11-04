using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    class Where
    {
        private List<List<Filtro>> listaFiltro; //A lista mais externa contém os agrupamentos de OU
        private List<Filtro> listaJoin;

        public Where()
        {
            listaFiltro = new List<List<Filtro>>();
            listaJoin = new List<Filtro>();

        }

        public void addJoin(Filtro join)
        {

        }

        public void addFiltroOR()
        {

        }

        public void addFiltroAND()
        {

        }

        internal List<List<Filtro>> ListaFiltro
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

        internal List<Filtro> ListaJoin
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

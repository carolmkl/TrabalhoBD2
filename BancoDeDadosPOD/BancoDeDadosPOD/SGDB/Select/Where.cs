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
        //o List interno contem o nome do campo filtrado (LValue) e o Filtro
        private List<Filtro> listaJoin;

        public Where()
        {
            listaFiltro = new List<List<Filtro>>();
            listaJoin = new List<Filtro>();

        }

        public void addJoin(Filtro join)
        {
            listaJoin.Add(join);
        }

        public void addFiltroOR(Filtro filtro)
        {
            //cria um novo bloco de consulta para agrupar o OU
            listaFiltro.Add(new List<Filtro>());
            //insere o filtro no último bloco criado
            listaFiltro[listaFiltro.Count - 1].Add(filtro);
        }

        public void addFiltroAND(Filtro filtro)
        {
            if(listaFiltro.Count == 0)
            {
                //caso nao exista nenhum bloco de consulta, cria
                listaFiltro.Add(new List<Filtro>());
            }
            //insere o filtro no último bloco criado
            listaFiltro[listaFiltro.Count - 1].Add(filtro);
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

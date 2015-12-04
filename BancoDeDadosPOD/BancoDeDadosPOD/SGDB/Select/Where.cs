using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Select
{
    class Where
    {
        //A lista mais externa contém os agrupamentos de OU
        //o List interno contem o agrupamento AND
        private List<List<Filtro>> listaFiltro; 

        private List<Filtro> listaJoin;
        
        //dictionary para incluir a faixa de valores para filtrar conforme Join.
        private Dictionary<string, List<string>> filtroJoin;

        public Where()
        {
            listaFiltro = new List<List<Filtro>>();
            listaJoin = new List<Filtro>();
            filtroJoin = new Dictionary<string, List<string>>();
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

        public Dictionary<string, List<string>> FiltroJoin
        {
            get
            {
                return filtroJoin;
            }

            set
            {
                filtroJoin = value;
            }
        }
    }
}

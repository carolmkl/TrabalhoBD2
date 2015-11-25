using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BancoDeDadosPOD.SGDB.Select
{
    class GambiarraSelect
    {
        private static GambiarraSelect instance;
        private GerenciadorMemoria mem;

        private GambiarraSelect()
        {
            mem = GerenciadorMemoria.getInstance();
        }

        public static GambiarraSelect getInstance()
        {
            instance = (instance == null) ? new GambiarraSelect() : instance;
            return instance;
        }

        public TabelaSelect returnDados(Metadados tabela)
        {

            return null;
        }

        public TabelaSelect returnDados(List<Filtro> filtrosAND, Metadados tabela)
        {
            string arquivo = mem.getPath() + "\\" + tabela.getNome();
            FileStream file = new FileStream(arquivo, FileMode.Open);
            BinaryReader br = new BinaryReader(file);
            //while(br.)





            return null;
        }

    }
}

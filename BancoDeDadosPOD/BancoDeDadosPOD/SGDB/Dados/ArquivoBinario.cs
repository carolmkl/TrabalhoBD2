using System.Collections.Generic;
using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    class ArquivoBinario
    {
        public ArquivoBinario(string path)
        {
            File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void gravarTudo(List<Registro> registros)
        {
            // aqui deve ser descarregada a tabela toda
        }

        public void gravarNovos()
        {
            // aqui devem ser gravados apenas o ultimos registros inseridos na tabela
        }

        public List<Registro> LerRegistros()
        {
            // aqui deverá ser carregado todos os registros de uma tabela da base de dados

            return null;
        }

        public MemoryStream Ler()
        {
            // se precisarmos desenvolverei este, q deverá ser hardcore.

            return null;
        }
    }
}
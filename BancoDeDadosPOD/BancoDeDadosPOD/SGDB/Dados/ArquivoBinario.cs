using System.Collections.Generic;
using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoBinario
    {
        public ArquivoBinario(string path)
        {
            File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public long insert(Registro registro)
        {
            foreach (Dado d in registro.dados)
            {

            }
        }
    }
}
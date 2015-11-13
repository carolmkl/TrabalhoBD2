using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoBinario
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;

        public ArquivoBinario(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
        }

        public long insert(Registro registro)
        {
            long posicaoIni = stream.Length;
            stream.Position = posicaoIni - 1;

            // Posição do registro
            bw.Write(posicaoIni);

            // Quantidade de colunas do registro
            bw.Write(registro.Dados.Count);

            // Dados do registro
            foreach (Dado d in registro.Dados)
            {
                // tamanho do dado de acordo com o metadados em bytes
                bw.Write(d.tamanho);

                // informa se o dado é valido
                bw.Write(d.isValido);

                // grava o dado no arquivo
                if (d.tipo == TipoDado.Inteiro)
                    bw.Write(d.getValorInt());
                else
                    bw.Write(d.getValorStr().PadRight(d.tamanho));
            }

            // força a gravar no arquivo aquilo que ficou no buffer.
            bw.Flush();

            return posicaoIni;
        }
    }
}
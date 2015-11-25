using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoTabela
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;
        string path;

        public ArquivoTabela(string path)
        {
            this.path = path;
            
        }

        public long insert(Registro registro)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);

            long posicaoIni = stream.Length;
            stream.Position = posicaoIni;

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
            br.Close();
            bw.Close();
            return posicaoIni;
        }

        public bool temDados()
        {
            return stream.Length > 0;
        }
    }

    public sealed class ArquivoSelect
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;
        string path;

        public ArquivoSelect(string path)
        {
            this.path = path;
        }

        public TabelaSelect returnTudo(string nome, string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);

            int count;
            TabelaDado td = new TabelaDado(nome, path);
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados()[nome];
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                Registro r = new Registro(br.ReadInt64());
                count = br.ReadInt32();
                Form1.addMensagem("Count colunas" + count);
                for (int i = 0; i < count; i++)
                {
                    Dado d;
                    if (meta.getDados()[meta.getNomesColunas()[i]].getTipoDado() == TipoDado.Inteiro)
                    {
                        Form1.addMensagem("Inteiro");
                        d = new Dado(meta.getNomesColunas()[i], meta.getDados()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadInt32());
                    } else
                    {
                        Form1.addMensagem("Char");
                        d = new Dado(meta.getNomesColunas()[i], meta.getDados()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadString());
                    }
                    

                    r.Dados.Add(d);
                }
                td.Registros.Add(r);
            }
            br.Close();
            bw.Close();
            return TabelaSelect.getTabelaSelect(td);

        }
    }

        public sealed class ArquivoIndice
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;
        string path;

        public ArquivoIndice(string path)
        {
            this.path = path;
        }

        public long insert(DadoIndice registro, long posicao)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);

            long posicaoIni = stream.Length;
            stream.Position = posicaoIni;

            /*
                primeiro posição do registro no arquivo da tabela, 
                para nao precisar saber o tamanho da chave se for string.
            */

            // insere a posicao do registro no arquivo da tabela
            bw.Write(posicao);

            // grava o dado no indice
            if (registro.tipo == TipoDado.Inteiro)
                bw.Write(registro.getValorInt());
            else
                bw.Write(registro.getValorStr());

            br.Close();
            bw.Close();
            return posicaoIni;
        }

        
    }
}
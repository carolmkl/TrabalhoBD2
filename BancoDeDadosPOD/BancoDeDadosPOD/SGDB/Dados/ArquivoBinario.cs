using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoTabela
    {
        private Stream stream;
        private BinaryWriter bw;
        private BinaryReader br;
        private string path;

        public ArquivoTabela(string path)
        {
            this.stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            this.bw = new BinaryWriter(stream);
            this.br = new BinaryReader(stream);
            this.path = path;
        }
        /*
        ~ArquivoTabela(string path)
        {
            this.br.Close();
            this.bw.Close();
            this.stream.Close();
        }
        */

        public long insert(Registro registro)
        {
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
            
            return posicaoIni;
        }

        public bool temDados()
        {
            return stream.Length > 0;
        }

        public TabelaSelect returnTudo(string nome, string path)
        {
            int count;
            TabelaDado td = new TabelaDado(nome, path);
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(nome);
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                Registro r = new Registro(br.ReadInt64());
                count = br.ReadInt32();
                //Form1.addMensagem("Count colunas" + count); *** para depuração
                for (int i = 0; i < count; i++)
                {
                    Dado d;
                    if (meta.getDadosColuna()[meta.getNomesColunas()[i]].getTipoDado() == TipoDado.Inteiro)
                    {
                        d = new Dado(meta.getNomesColunas()[i], meta.getDadosColuna()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadInt32());
                        // Form1.addMensagem("Inteiro " + d.getValorInt()); *** para depuração
                    }
                    else
                    {
                        d = new Dado(meta.getNomesColunas()[i], meta.getDadosColuna()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadString());
                        //  Form1.addMensagem("Char " + d.getValorStr()); *** para depuração
                    }

                    r.Dados.Add(d);
                }
                td.Registros.Add(r);
            }

            return TabelaSelect.getTabelaSelect(td);
        }
    }

    public sealed class ArquivoIndice
    {
        private Stream stream;
        private BinaryReader br;
        private BinaryWriter bw;
        private string path;

        public ArquivoIndice(string path)
        {
            this.stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            this.br = new BinaryReader(stream);
            this.bw = new BinaryWriter(stream);
            this.path = path;
        }

        ~ArquivoIndice()
        {
            this.br.Close();
            this.bw.Close();
            this.stream.Close();
        }

        public long insert(DadoIndice registro, long posicao)
        {
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

            bw.Flush();

            return posicaoIni;
        }
    }
}
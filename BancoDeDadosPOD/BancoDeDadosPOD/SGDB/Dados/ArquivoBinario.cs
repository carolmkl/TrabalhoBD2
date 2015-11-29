using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoTabela
    {
        public string nome { get; internal set; }
        public string path { get; internal set; }
        private Stream stream;
        private BinaryWriter bw;
        private BinaryReader br;

        #region *** Construtor e Destrutor ***
        public ArquivoTabela(string nome)
        {
            this.nome = nome;
            this.stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            this.bw = new BinaryWriter(stream);
            this.br = new BinaryReader(stream);
        }
        
        ~ArquivoTabela()
        {
            this.br.Close();
            this.bw.Close();
            this.stream.Close();
        }
        #endregion

        public bool temDados()
        {
            return stream.Length > 0;
        }

        public long insert(RegistroTabela RegistroTabela)
        {
            long posicaoIni = stream.Length;
            stream.Position = posicaoIni;

            // Posição do RegistroTabela
            bw.Write(posicaoIni);

            // Quantidade de colunas do RegistroTabela
            bw.Write(RegistroTabela.dados.Count);

            // Dados do RegistroTabela
            foreach (DadoTabela d in RegistroTabela.dados)
            {
                // tamanho do DadoTabela de acordo com o metadados em bytes
                bw.Write(d.tamanho);

                // informa se o DadoTabela é valido
                bw.Write(d.isValido);

                // grava o DadoTabela no arquivo
                if (d.tipo == TipoDado.Inteiro)
                    bw.Write(d.getValorInt());
                else
                {
                    byte[] valor = new byte[d.tamanho];
                    new System.Text.ASCIIEncoding().GetBytes(d.getValorStr().PadRight(d.tamanho)).CopyTo(valor,0);

                    bw.Write(valor);
                    //bw.Write(d.getValorStr().PadRight(d.tamanho));
                }
            }

            // força a gravar no arquivo aquilo que ficou no buffer.
            bw.Flush();

            return posicaoIni;
        }

        public TabelaSelect returnTudo(string nome, string path)
        {
            int count;
            TabelaDado td = new TabelaDado(nome, path);
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(nome);
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                RegistroTabela r = new RegistroTabela(br.ReadInt64());
                count = br.ReadInt32();
                //Form1.addMensagem("Count colunas" + count); // somente para depuração
                for (int i = 0; i < count; i++)
                {
                    DadoTabela d;
                    if (meta.getDados()[meta.getNomesColunas()[i]].getTipoDado() == TipoDado.Inteiro)
                    {
                        //Form1.addMensagem("Inteiro"); // somente para depuração
                        d = new DadoTabela(meta.getNomesColunas()[i], meta.getDados()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadInt32());
                    }
                    else
                    {
                        //Form1.addMensagem("Char"); // somente para depuração
                        d = new DadoTabela(meta.getNomesColunas()[i], meta.getDados()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(), br.ReadString());
                    }

                    r.dados.Add(d);
                }

                td.registros.Add(r);
            }

            return TabelaSelect.getTabelaSelect(td);
        }
    }

    public sealed class ArquivoIndice
    {
        public string nome { get; internal set; }
        public string path { get; internal set; }
        public Stream stream;
        public BinaryWriter bw;
        public BinaryReader br;

        #region *** Construtor e Destrutor ****
        public ArquivoIndice(string nome)
        {
            this.nome = nome;
            this.stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            this.bw = new BinaryWriter(stream);
            this.br = new BinaryReader(stream);
        }

        ~ArquivoIndice()
        {
            this.br.Close();
            this.bw.Close();
            this.stream.Close();
        }
        #endregion

        public bool temDados()
        {
            return stream.Length > 0;
        }

        public long insert(RegistroIndice registro, long posicao)
        {
            long posicaoIni = stream.Length;
            stream.Position = posicaoIni;

            /*
                primeiro posição do RegistroTabela no arquivo da tabela, 
                para nao precisar saber o tamanho da chave se for string.
            */

            // insere a posicao do RegistroTabela no arquivo da tabela
            bw.Write(posicao);

            // Dados do RegistroTabela
            foreach (DadoIndice d in registro.dados)
            {
                // posicao ordinal do campo dentro da tabela
                bw.Write(d.posicao);

                // grava o DadoTabela no indice
                if (d.tipo == TipoDado.Inteiro)
                    bw.Write(d.getValorInt());
                else
                    bw.Write(d.getValorStr());
            }

            // força a gravar no arquivo aquilo que ficou no buffer.
            bw.Flush();

            return posicaoIni;
        }
    }
}
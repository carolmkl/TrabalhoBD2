using BancoDeDadosPOD.SGDB.Select;
using BD2.Analizadores;
using System;
using System.Collections.Generic;
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
        private long posicaoIni;

        #region *** Construtor e Destrutor ***
        public ArquivoTabela(string nome)
        {
            this.nome = nome;
            this.path = GerenciadorMemoria.getInstance().getPath() + "\\" + nome + ".dat";
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

        public void desalocar()
        {
            this.br.Dispose();
            this.bw.Dispose();
            this.stream.Dispose();
        }

        public bool temDados()
        {
            return stream.Length > 0;
        }

        private void atualizarPosicaoIni()
        {
            posicaoIni = stream.Length;

            if (stream.Position != posicaoIni)
                stream.Position = posicaoIni;
        }

        public long insert(RegistroTabela RegistroTabela)
        {
            atualizarPosicaoIni();

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
                    // qual o problema aqui?? testeis as duas formas antes de fazer.
                    // deixe sua resposta aqui.

                    //byte[] valor = new byte[d.tamanho];
                    //new System.Text.ASCIIEncoding().GetBytes(d.getValorStr().PadRight(d.tamanho)).CopyTo(valor,0);
                    //bw.Write(valor);

                    bw.Write(d.getValorStr().PadRight(d.tamanho));
                }
            }

            // força a gravar no arquivo aquilo que ficou no buffer.
            bw.Flush();

            return posicaoIni;
        }

        public TabelaSelect returnTudo()
        {
            br.BaseStream.Position = 0;
            int count;
            TabelaDado td = new TabelaDado(nome);
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(nome);
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                RegistroTabela r = new RegistroTabela(br.ReadInt64());
                count = br.ReadInt32();
                //Form1.addMensagem("Count colunas" + count); // somente para depuração

                try {
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
                } catch(System.Exception e)
                {
                   Form1.addMensagem(e.Message);
                }

                td.registros.Add(r);
            }

            return TabelaSelect.getTabelaSelect(td);
        }

        public TabelaSelect returnFiltrado(Dictionary<string, Filtro> filtrosAND)
        {
            br.BaseStream.Position = 0;
            int count;
            int tamRegistro = 12;
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(nome);
            TabelaDado td = new TabelaDado(nome);
                
            foreach (DadosTabela dados in meta.getDados().Values)
            {
                tamRegistro += dados.getTamanho() + 2;
                //if (dados.getTipoDado() == TipoDado.String) tamRegistro++;
            }

            //lê cada registro
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                RegistroTabela r = new RegistroTabela(br.BaseStream.Position);
                count = br.ReadInt32();
                bool insere = true;
                //Lê cada dado dentro do registro
                for (int i = 0; i < count && insere; i++)
                {
                    DadoTabela d;
                    //Form1.addMensagem(i.ToString());

// *** erro aqui - inicio ***
                    // select localidade.* from localidade where localidade.cd_localidade = 1;
                    // da erro qdo i = 4
                    // mas quem deve limitar para nao chegar no 4 ?

                    string nomeColuna = meta.getNomesColunas()[i];
// *** erro aqui - Fim ***

                    TipoDado tipo = meta.getDados()[nomeColuna].getTipoDado();
                    string campo = meta.getNome() + "." + nomeColuna;
                    Filtro f = filtrosAND.ContainsKey(campo) ? filtrosAND[campo] : null;
                    if (tipo == TipoDado.Inteiro)
                    {
                        d = new DadoTabela(nomeColuna, tipo, br.ReadByte(), br.ReadBoolean(), br.ReadInt32());
                        if (f != null)
                        {
                            switch (f.Op)
                            {
                                case OperadorRel.Igual:
                                    if (f.RValue.ToLower().Equals("null"))
                                    {
                                        if (d.isValido) insere = false;
                                    }
                                    else
                                    {
                                        if (d.getValorInt() != Convert.ToInt32(f.RValue)) insere = false;
                                    }
                                    break;
                                case OperadorRel.MaiorQue:
                                    if (d.getValorInt() <= Convert.ToInt32(f.RValue)) insere = false;
                                    break;
                                case OperadorRel.MenorQue:
                                    if (d.getValorInt() >= Convert.ToInt32(f.RValue)) insere = false;
                                    break;
                                case OperadorRel.MaiorIgualA:
                                    if (d.getValorInt() < Convert.ToInt32(f.RValue)) insere = false;
                                    break;
                                case OperadorRel.MenorIgualA:
                                    if (d.getValorInt() > Convert.ToInt32(f.RValue)) insere = false;
                                    break;
                                case OperadorRel.Diferente:
                                    if (f.RValue.ToLower().Equals("null"))
                                    {
                                        if (!d.isValido) insere = false;
                                    }
                                    else
                                    {
                                        if (d.getValorInt() == Convert.ToInt32(f.RValue)) insere = false;
                                    }
                                    break;
                                default:
                                    throw new SGDBException("Passou onde nao devia: GambiarraSelect.retornaDados.Inteiro.Default.");
                            }
                        }
                    }
                    else
                    {
                        byte tamanho = br.ReadByte();
                        bool isValido = br.ReadBoolean();
                        byte[] valor = br.ReadBytes(tamanho);
                        string texto = new System.Text.ASCIIEncoding().GetString(valor);
                        d = new DadoTabela(nomeColuna, tipo, tamanho, isValido, texto);
                        if (f != null)
                        {
                            switch (f.Op)
                            {
                                case OperadorRel.Igual:
                                    if (f.RValue.ToLower().Equals("null"))
                                    {
                                        if (d.isValido) insere = false;
                                    }
                                    else
                                    {
                                        byte[] filtro = new byte[d.tamanho];
                                        new System.Text.ASCIIEncoding().GetBytes(f.RValue.PadRight(d.tamanho)).CopyTo(filtro, 0);
                                        string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro);
                                        if (!texto.Equals(filtro2)) insere = false;
                                    }
                                    break;
                                case OperadorRel.Diferente:
                                    if (f.RValue.ToLower().Equals("null"))
                                    {
                                        if (!d.isValido) insere = false;
                                    }
                                    else
                                    {
                                        byte[] filtro = new byte[d.tamanho];
                                        new System.Text.ASCIIEncoding().GetBytes(f.RValue.PadRight(d.tamanho)).CopyTo(filtro, 0);
                                        string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro);
                                        if (texto.Equals(filtro2)) insere = false;
                                    }
                                    break;
                                default:
                                    throw new SemanticError("Comparação de literais só pode ser igual ou diferente");
                            }
                        }
                    }

                    r.dados.Add(d);
                }

                if (insere)
                {
                    td.registros.Add(r);
                }

                if (br.BaseStream.Position % tamRegistro != 0)
                    br.BaseStream.Position += tamRegistro - (br.BaseStream.Position % tamRegistro);
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
            this.path = GerenciadorMemoria.getInstance().getPath() + "\\" + nome + ".idx";
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

        public void desalocar()
        {
            this.br.Dispose();
            this.bw.Dispose();
            this.stream.Dispose();
        }

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
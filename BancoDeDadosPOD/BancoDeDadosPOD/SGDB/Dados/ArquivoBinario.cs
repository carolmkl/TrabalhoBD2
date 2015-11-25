﻿using System.IO;

namespace BancoDeDadosPOD.SGDB.Dados
{
    public sealed class ArquivoTabela
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;

        public ArquivoTabela(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
        }

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
    }

    public sealed class ArquivoSelect
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;

        public ArquivoSelect(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
        }

        public TabelaSelect returnTudo(string nome, string path)
        {
            int count;
            TabelaDado td = new TabelaDado(nome, path);
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados()[nome];
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                Registro r = new Registro(br.Read());
                count = br.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Dado d = new Dado(meta.getNomesColunas()[i],meta.getDados()[meta.getNomesColunas()[i]].getTipoDado(), br.ReadByte(), br.ReadBoolean(),br.Read());

                    r.Dados.Add(d);
                }
                td.Registros.Add(r);
            }

            return TabelaSelect.getTabelaSelect(td);

        }
    }

        public sealed class ArquivoIndice
    {
        Stream stream;
        BinaryWriter bw;
        BinaryReader br;

        public ArquivoIndice(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
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

            return posicaoIni;
        }

        
    }
}
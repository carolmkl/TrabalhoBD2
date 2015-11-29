using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using BancoDeDadosPOD.SGDB.Dados;
using BD2.Analizadores;

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

        public TabelaSelect returnDados(Dictionary<string, Filtro> filtrosAND, Metadados tabela)
        {
            TabelaDado td;
            BinaryReader br = null;
            try
            {
                string arquivo = mem.getPath() + "\\" + tabela.getNome() + ".dat";
                FileStream file = new FileStream(arquivo, FileMode.Open);
                br = new BinaryReader(file);

                int count;
                td = new TabelaDado(tabela.getNome(), arquivo);
                Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(tabela.getNome());
                int tamRegistro = 12;
                foreach (DadosTabela dados in meta.getDados().Values)
                {
                    tamRegistro += dados.getTamanho() + 2;
                    //if (dados.getTipoDado() == TipoDado.String) tamRegistro++;
                }
                //lê cada registro
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    RegistroTabela r = new RegistroTabela(br.ReadInt64());
                    count = br.ReadInt32();
                    bool insere = true;
                    //Lê cada dado dentro do registro
                    for (int i = 0; i < count && insere; i++)
                    {
                        DadoTabela d;
                        string nomeColuna = meta.getNomesColunas()[i];
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

            }
            finally
            {
                br.Close();
            }
            return TabelaSelect.getTabelaSelect(td);

        }

    }
}

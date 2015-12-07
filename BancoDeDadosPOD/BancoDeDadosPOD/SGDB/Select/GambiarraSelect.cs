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
    /// <summary>
    /// métodos transferidos para a classe Binarios, no arquivo SGDB/Dados/Base.cs
    /// mantendo a classe para registro...
    /// </summary>
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
            try { Base.getInstance().desalocarBinarios(tabela.getNome()); } catch { }
            TabelaSelect ts = null;
            FileStream file = null;
            BinaryReader br = null;
            try
            {
                string arquivo = mem.getPath() + "\\" + tabela.getNome() + ".dat";
                file = new FileStream(arquivo, FileMode.Open);
                using (br = new BinaryReader(file))
                {
                    int count;
                    ts = new TabelaSelect();

                    Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(tabela.getNome());
                    int colunas = meta.getNomesColunas().Count;
                    ts.Campos = new string[colunas];
                    for (int i = 0; i < colunas; i++)
                    {
                        ts.Campos[i] = meta.getNome() + "." + meta.getNomesColunas()[i];
                    }

                    //lê cada registro
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        string[] registro = new string[colunas];
                        RegistroTabela r = new RegistroTabela(br.ReadInt64());
                        count = br.ReadInt32();
                        //Lê cada dado dentro do registro
                        for (int i = 0; i < count; i++)
                        {
                            string nomeColuna = meta.getNomesColunas()[i];
                            TipoDado tipo = meta.getDados()[nomeColuna].getTipoDado();
                            string valor = "";
                            if (tipo == TipoDado.Inteiro)
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                int numero = br.ReadInt32();
                                valor = isValido ? numero + "" : "NULL";
                            }
                            else
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                byte[] literal = br.ReadBytes(tamanho);
                                string texto = new System.Text.ASCIIEncoding().GetString(literal);
                                valor = isValido ? texto : "NULL";
                            }

                            registro[i] = valor;
                        }

                        ts.Registros.Add(registro);
                        if (ts.Registros.Count >= Base.getInstance().qtd_max_registros) break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (file != null)
                {
                    file.Close();
                }
            }

            return ts;
        }
        public TabelaSelect returnDados(List<Filtro> filtrosAND, Dictionary<string, List<string>> filtrosJoin, Metadados tabela)
        {
            try { Base.getInstance().desalocarBinarios(tabela.getNome()); } catch { }
            TabelaSelect ts = null;
            FileStream file = null;
            BinaryReader br = null;
            try
            {
                string arquivo = mem.getPath() + "\\" + tabela.getNome() + ".dat";
                file = new FileStream(arquivo, FileMode.Open);
                using (br = new BinaryReader(file))
                {
                    int count;
                    ts = new TabelaSelect();

                    Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(tabela.getNome());
                    int colunas = meta.getNomesColunas().Count;
                    ts.Campos = new string[colunas];
                    for (int i = 0; i < colunas; i++)
                    {
                        ts.Campos[i] = meta.getNome() + "." + meta.getNomesColunas()[i];
                    }

                    //calcula o tamanho de cada registro
                    int tamRegistro = 12;
                    int posPrimary = 0;//posicao do primary key no registro
                    foreach (DadosTabela dados in meta.getDados().Values)
                    {
                        if (dados.isPrimary()) posPrimary = tamRegistro + 2;
                        tamRegistro += dados.getTamanho() + 2;
                    }

                    long posMax = br.BaseStream.Length; //posicao máxima para leitura do arquivo
                    //organiza os filtros por coluna
                    List<Filtro>[] filtrosCampo = new List<Filtro>[colunas];
                    for (int i = 0; i < colunas; i++)
                    {
                        ts.Campos[i] = meta.getNome() + "." + meta.getNomesColunas()[i];
                        filtrosCampo[i] = new List<Filtro>();
                        foreach (Filtro f in filtrosAND)
                        {
                            if (f.LValue.Equals(ts.Campos[i])) filtrosCampo[i].Add(f);
                        }
                        if (filtrosJoin.ContainsKey(ts.Campos[i]))
                        {
                            posMax = posicionaPonteiroArquivo(filtrosJoin[ts.Campos[i]], br, tamRegistro, posPrimary);
                        }
                        //se o campo for PrimaryKey organiza o filtro
                        else if (filtrosCampo[i].Count > 0 && meta.getDados()[meta.getNomesColunas()[i]].isPrimary())
                        {
                            ordenaFiltros(filtrosCampo[i]);
                            //define o intervalo de consulta do arquivo caso exista filtro de chave primaria
                            posMax = posicionaPonteiroArquivo(filtrosCampo[i], br, tamRegistro, posPrimary);
                        }
                    }

                    //lê cada registro
                    while (br.BaseStream.Position < posMax)
                    {
                        string[] registro = new string[colunas];
                        long posicao = br.ReadInt64();
                        count = br.ReadInt32();
                        bool insere = true;
                        //Lê cada dado dentro do registro
                        for (int i = 0; i < count && insere; i++)
                        {
                            string nomeColuna = meta.getNomesColunas()[i];
                            TipoDado tipo = meta.getDados()[nomeColuna].getTipoDado();
                            bool isPrimary = meta.getDados()[nomeColuna].isPrimary();
                            string valor = "";
                            string campo = meta.getNome() + "." + nomeColuna;

                            if (tipo == TipoDado.Inteiro)
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                int numero = br.ReadInt32();
                                valor = isValido ? numero + "" : "NULL";
                                foreach (Filtro f in filtrosCampo[i])
                                {
                                    switch (f.Op)
                                    {
                                        case OperadorRel.Igual:
                                            if (f.RValue.ToLower().Equals("null"))
                                            {
                                                if (isValido) insere = false;
                                            }
                                            else
                                            {
                                                if (numero != Convert.ToInt32(f.RValue)) insere = false;
                                            }
                                            break;
                                        case OperadorRel.MaiorQue:
                                            if (numero <= Convert.ToInt32(f.RValue)) insere = false;
                                            break;
                                        case OperadorRel.MenorQue:
                                            if (numero >= Convert.ToInt32(f.RValue)) insere = false;
                                            break;
                                        case OperadorRel.MaiorIgualA:
                                            if (numero < Convert.ToInt32(f.RValue)) insere = false;
                                            break;
                                        case OperadorRel.MenorIgualA:
                                            if (numero > Convert.ToInt32(f.RValue)) insere = false;
                                            break;
                                        case OperadorRel.Diferente:
                                            if (f.RValue.ToLower().Equals("null"))
                                            {
                                                if (!isValido) insere = false;
                                            }
                                            else
                                            {
                                                if (numero == Convert.ToInt32(f.RValue)) insere = false;
                                            }
                                            break;
                                        default:
                                            throw new SGDBException("Passou onde nao devia: GambiarraSelect.retornaDados.Inteiro.Default.");
                                    }
                                }
                                if(insere && filtrosJoin.ContainsKey(campo))
                                {
                                    insere = filtrosJoin[campo].Exists(s => s.Equals(valor));
                                }
                            }
                            else
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                byte[] literal = br.ReadBytes(tamanho);
                                string texto = new System.Text.ASCIIEncoding().GetString(literal);
                                valor = isValido ? texto.TrimEnd() : "NULL";
                                foreach (Filtro f in filtrosCampo[i])
                                {
                                    switch (f.Op)
                                    {
                                        case OperadorRel.Igual:
                                            if (f.RValue.ToLower().Equals("null"))
                                            {
                                                if (isValido) insere = false;
                                            }
                                            else
                                            {
                                                byte[] filtro = new byte[tamanho];
                                                new System.Text.ASCIIEncoding().GetBytes(f.RValue.PadRight(tamanho)).CopyTo(filtro, 0);
                                                string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro).TrimEnd();
                                                if (!valor.Equals(filtro2)) insere = false;
                                            }
                                            break;
                                        case OperadorRel.Diferente:
                                            if (f.RValue.ToLower().Equals("null"))
                                            {
                                                if (isValido) insere = false;
                                            }
                                            else
                                            {
                                                byte[] filtro = new byte[tamanho];
                                                new System.Text.ASCIIEncoding().GetBytes(f.RValue.PadRight(tamanho)).CopyTo(filtro, 0);
                                                string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro).TrimEnd();
                                                if (valor.Equals(filtro2)) insere = false;
                                            }
                                            break;
                                        default:
                                            throw new SemanticError("Comparação de literais só pode ser igual ou diferente");
                                    }
                                }
                                if (insere && filtrosJoin.ContainsKey(campo))
                                {
                                    insere = filtrosJoin[campo].Exists(s => s.Equals(valor));
                                }

                            }

                            registro[i] = valor;
                        }

                        if (insere)
                        {
                            ts.Registros.Add(registro);
                            if (ts.Registros.Count >= Base.getInstance().qtd_max_registros) break;
                        }

                        if (br.BaseStream.Position % tamRegistro != 0)
                            br.BaseStream.Position += tamRegistro - (br.BaseStream.Position % tamRegistro);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (file != null)
                {
                    file.Close();
                }
            }
            return ts;
        }

        /// <summary>
        /// Como os inserts no banco de dados estão ordenados por Primary Key
        /// caso seja feita um select filtrando por este, uma consulta binária é realizada.
        /// Posiciona o ponteiro do buffer no local inicial e retorna a posicao máxima para busca
        /// </summary>
        /// <param name="list"></param>
        /// <param name="br"></param>
        /// <param name="tamRegistro"></param>
        /// <param name="posPrimary"></param>
        private long posicionaPonteiroArquivo(List<Filtro> list, BinaryReader br, int tamRegistro, int posPrimary)
        {
            long posMin = 0; //para posicionar o binario
            long posMax = br.BaseStream.Length; //para retorno
            if (list.First().Op == OperadorRel.MaiorIgualA || list.First().Op == OperadorRel.MaiorQue || list.First().Op == OperadorRel.Igual)
            {
                posMin = getPosicaoMin(Convert.ToInt32(list.First().RValue), br, tamRegistro, posPrimary, posMax);
            }
            else
            {
                foreach (Filtro f in list)
                {
                    if (f.Op == OperadorRel.Igual)
                    {
                        posMin = getPosicaoMin(Convert.ToInt32(list.First().RValue), br, tamRegistro, posPrimary, posMax);
                        break;
                    }
                }
            }
            if (list.Last().Op == OperadorRel.MaiorIgualA || list.Last().Op == OperadorRel.MaiorQue || list.Last().Op == OperadorRel.Igual)
            {
                posMax = getPosicaoMax(Convert.ToInt32(list.Last().RValue), br, tamRegistro, posPrimary, posMin);
            }
            else
            {
                for (int i = list.Count; i > -1; i--)
                {
                    if (list[i].Op == OperadorRel.Igual)
                    {
                        posMax = getPosicaoMax(Convert.ToInt32(list.Last().RValue), br, tamRegistro, posPrimary, posMin);
                        break;
                    }
                }
            }

            br.BaseStream.Position = posMin;
            return posMax;
        }

        /// <summary>
        /// Como os inserts no banco de dados estão ordenados por Primary Key
        /// caso seja feita um select filtrando por este, uma consulta binária é realizada.
        /// Posiciona o ponteiro do buffer no local inicial e retorna a posicao máxima para busca
        /// </summary>
        /// <param name="list"></param>
        /// <param name="br"></param>
        /// <param name="tamRegistro"></param>
        /// <param name="posPrimary"></param>
        private long posicionaPonteiroArquivo(List<string> list, BinaryReader br, int tamRegistro, int posPrimary)
        {
            long posMin = 0; //para posicionar o binario
            long posMax = br.BaseStream.Length; //para retorno
            posMin = getPosicaoMin(Convert.ToInt32(list.First()), br, tamRegistro, posPrimary, posMax);
            posMax = getPosicaoMax(Convert.ToInt32(list.Last()), br, tamRegistro, posPrimary, posMin);

            br.BaseStream.Position = posMin;
            return posMax;
        }

        /// <summary>
        /// busca binaria pra retornar a posicao minima
        /// </summary>
        /// <param name="valor">valor para buscar</param>
        /// <param name="br"></param>
        /// <param name="tamRegistro"></param>
        /// <param name="posPrimary"></param>
        /// <param name="max">posicao maxima que o min pode atingir</param>
        /// <returns></returns>
        private long getPosicaoMin(int valor, BinaryReader br, int tamRegistro, int posPrimary, long max)
        {
            long meio;
            long min = 0;
            do
            {
                meio = (int)(min + max) / 2;
                //coloca o ponteiro no inicio do registro, caso nao esteja
                if (meio % tamRegistro != 0)
                    meio += tamRegistro - (meio % tamRegistro);
                br.BaseStream.Position = meio + posPrimary; //pula pra posicao do PrimaryKey no arquivo
                int valorLido = br.ReadInt32();
                if (valorLido == valor)
                {
                    return meio;
                }
                else if (valorLido < valor)
                {
                    min = meio + tamRegistro;
                }
                else
                {
                    max = meio - 1;
                }
            } while (min + tamRegistro < max);
            return min;
        }


        /// <summary>
        /// busca binaria pra retornar a posicao maxima
        /// </summary>
        /// <param name="valor para buscar"></param>
        /// <param name="br"></param>
        /// <param name="tamRegistro"></param>
        /// <param name="posPrimary"></param>
        /// <param name="min">posicao minima que o maximo pode atingir</param>
        /// <returns></returns>
        private long getPosicaoMax(int valor, BinaryReader br, int tamRegistro, int posPrimary, long min)
        {
            long meio;
            long max = br.BaseStream.Length;
            do
            {
                meio = (int)(min + max) / 2;
                //coloca o ponteiro no inicio do registro, caso nao esteja
                if (meio % tamRegistro != 0)
                    meio += tamRegistro - (meio % tamRegistro);
                br.BaseStream.Position = meio + posPrimary; //pula pra posicao do PrimaryKey no arquivo
                int valorLido = br.ReadInt32();
                if (valorLido == valor)
                {
                    return meio + tamRegistro;
                }
                else if (valorLido < valor)
                {
                    min = meio + tamRegistro;
                }
                else
                {
                    max = meio - 1;
                }
            } while (min + tamRegistro < max);
            return max + tamRegistro;
        }

        private void ordenaFiltros(List<Filtro> filtros)
        {
            filtros.Sort(delegate (Filtro f1, Filtro f2)
            {
                if (f1.Op == f2.Op)
                    return Convert.ToInt32(f1.RValue) > Convert.ToInt32(f2.RValue) ? -1 : 1;

                if (f1.Op == OperadorRel.MaiorIgualA || f1.Op == OperadorRel.MaiorQue
                || f2.Op == OperadorRel.MenorIgualA || f2.Op == OperadorRel.MenorQue)
                    return 1;

                if (f1.Op == OperadorRel.MenorIgualA || f1.Op == OperadorRel.MenorQue
                || f2.Op == OperadorRel.MaiorIgualA || f2.Op == OperadorRel.MaiorQue)
                    return -1;

                return Convert.ToInt32(f1.RValue) > Convert.ToInt32(f2.RValue) ? -1 : 1;
            });
        }

    }
}
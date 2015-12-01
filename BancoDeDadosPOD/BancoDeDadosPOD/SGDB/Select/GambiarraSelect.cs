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
        public TabelaSelect returnDados(List<Filtro> filtrosAND, Metadados tabela)
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
                    int tamRegistro = 12;
                    foreach (DadosTabela dados in meta.getDados().Values)
                    {
                        tamRegistro += dados.getTamanho() + 2;
                    }

                    //lê cada registro
                    while (br.BaseStream.Position != br.BaseStream.Length)
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
                            string valor = "";
                            string campo = meta.getNome() + "." + nomeColuna;

                            List<Filtro> fCampo = new List<Filtro>();
                            foreach (Filtro f in filtrosAND)
                            {
                                if (f.LValue.Equals(campo)) fCampo.Add(f);
                            }
                            if (tipo == TipoDado.Inteiro)
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                int numero = br.ReadInt32();
                                valor = isValido ? numero + "" : "NULL";
                                foreach (Filtro f in fCampo)
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
                            }
                            else
                            {
                                byte tamanho = br.ReadByte();
                                bool isValido = br.ReadBoolean();
                                byte[] literal = br.ReadBytes(tamanho);
                                string texto = new System.Text.ASCIIEncoding().GetString(literal);
                                valor = isValido ? texto.TrimEnd() : "NULL";
                                foreach (Filtro f in fCampo)
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
                                                string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro);
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
                                                string filtro2 = new System.Text.ASCIIEncoding().GetString(filtro);
                                                if (valor.Equals(filtro2)) insere = false;
                                            }
                                            break;
                                        default:
                                            throw new SemanticError("Comparação de literais só pode ser igual ou diferente");
                                    }
                                }
                            }

                            registro[i] = valor;
                        }

                        if (insere)
                        {
                            ts.Registros.Add(registro);
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
    }
}
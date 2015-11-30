using System;
using System.Collections.Generic;

namespace BancoDeDadosPOD.SGDB.Dados
{
    /*
    Metodos necessarios para select
    tabelaSelect returnDados(String tabela) //select * from tabela;
    tabelaSelect returnDados(List<string> colunas, string tabela) //select tabela.campo1 from tabela;
    tabelaSelect returnDados(List<Filtro> filtro, List<string> colunas, string tabela) //select tabela.campo1 from tabela where tabela.campo1 = 0 AND ...;

    Evoluiu para:
    TabelaSelect returnDados(Metadados tabela)
    private TabelaSelect returnDados(List<Filtro> filtrosAND, Metadados tabela)

    metodos necessario para Evandro
    salvarIndice(string[] valor, int lastPosi); - o nome já diz ele salva os dados com a posição no índice
    tabelaTemDados(string nomeTabela); - true se a tabela tem dados, false se não tiver
    int inserirDado(TabelaDado tabelaDado) - vai inserir o dado no arquivo da tabela;
    */
    public sealed class Base
    {
        private static Base instanciaUnica;
        public Dictionary<string, Binarios> arqBinarios;

        #region *** Construtores ***
        private Base()
        {
            arqBinarios = new Dictionary<string, Binarios>();
            carregarBinarios();
        }
        #endregion

        #region *** Getters e Setters ***
        public static Base getInstance()
        {
            if (instanciaUnica == null)
                instanciaUnica = new Base();

            if (instanciaUnica.arqBinarios.Count == 0)
                instanciaUnica.carregarBinarios();

            return instanciaUnica;
        }
        #endregion

        public void carregarBinarios()
        {
            if (GerenciadorMemoria.getInstance().metadados != null)
            {
                foreach (KeyValuePair<string, Metadados> item in GerenciadorMemoria.getInstance().metadados)
                {
                    arqBinarios.Add(item.Key, new Binarios(item.Key));
                }
            }
        }

        // Insere o registro na tabela e nos indices.
        public void insert(string tabela, RegistroTabela registro)
        {
            Binarios binAux = arqBinarios[tabela];
            long posicao = binAux.insertTabela(registro);
            binAux.insertIndices(registro, posicao, tabela);
        }

        public TabelaSelect returnDados(string tabela)
        // Isto devera mudar para ser independente de 1 ou todos os registros
        {
            return arqBinarios[tabela].select();
        }

        // Desalocar recursos para permitir alterações diretas no arquivo.
        public bool desalocarBinarios(string tabela)
        {
            arqBinarios[tabela].desalocarTabela();
            arqBinarios[tabela].desalocarIndices();

            return arqBinarios.Remove(tabela);
        }
    }

    public sealed class Binarios
    {
        private Memoria memoria { get; }
        private ArquivoTabela arqTabela { get; }
        private Dictionary<string, ArquivoIndice> arqsIndices { get; }

        #region *** Construtores ***
        public Binarios(string nomeTabela)
        {
            this.memoria = new Memoria();
            this.arqTabela = new ArquivoTabela(nomeTabela);
            this.arqsIndices = new Dictionary<string, ArquivoIndice>();

            carregarIndices();
        }
        #endregion

        // Carregar os arquivos de indices na memoria.
        private void carregarIndices()
        {
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(arqTabela.nome);
            foreach (KeyValuePair<string, string[]> item in meta.getIndexes())
            {
                arqsIndices.Add(item.Key, new ArquivoIndice(item.Key));
            }
        }

        // Insere um registroindice.
        private void insertIndice(string indice, RegistroIndice registro, long posicao)
        {
            try {
                arqsIndices[indice].insert(registro, posicao);
            } catch (Exception e) {
                throw new SGDBException("Houve erro na inserção do indice! " + e.Message);
            }
        }

        // Insere o registro nos indices.
        public void insertIndices(RegistroTabela registro, long posicao, string tabela)
        {
            if (arqsIndices.Count > 0)
            {
                // Percorre indices.
                RegistroIndice registroIndice;
                Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(tabela);
                foreach (KeyValuePair<string, string[]> item in meta.getIndexes())
                {
                    // Monta indice.
                    registroIndice = new RegistroIndice();
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        DadoIndice dadoIndice = new DadoIndice(meta.getDados()[item.Value[i]].getTipoDado(), registro.dados[meta.getNomesColunas().IndexOf(item.Value[i])].valor);
                        registroIndice.dados.Add(dadoIndice);
                    }

                    // Insere no indice.
                    insertIndice(item.Key, registroIndice, posicao);
                }
            }
        }

        public long insertTabela(RegistroTabela registro)
        {
            try {
                return arqTabela.insert(registro);
            } catch (Exception e) {
                throw new SGDBException("Houve erro na inserção do registro! " + e.Message);
            }
        }

        // Desalocar recursos para permitir alterações diretas no arquivo.
        public void desalocarIndices()
        {
            foreach (KeyValuePair<string, ArquivoIndice> item in arqsIndices)
                item.Value.desalocar();
        }

        // Desalocar recursos para permitir alterações diretas no arquivo.
        public void desalocarTabela()
        {
            arqTabela.desalocar();
        }

        public TabelaSelect select()
        // Isto devera mudar para ser independente de 1 ou todos os registros
        {
            //string arqTabela = GerenciadorMemoria.getInstance().getPath() + "\\" + tabela.getNome() + ".dat";
            //return new ArquivoTabela(arqTabela).returnTudo(tabela.getNome(), arqTabela);
            TabelaSelect retorno = arqTabela.returnTudo();
            return retorno;
        }
    }
}
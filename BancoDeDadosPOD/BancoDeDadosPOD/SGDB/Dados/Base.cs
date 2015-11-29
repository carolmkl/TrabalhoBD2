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
    public sealed class Binarios
    {
        private Memoria memoria { get; }
        private ArquivoTabela arqTabela { get; }
        private Dictionary<string, ArquivoIndice> arqsIndices { get; }

        #region *** Construtores ***
        private Binarios(string nomeTabela)
        {
            this.memoria = new Memoria();
            this.arqTabela = new ArquivoTabela(nomeTabela);
            this.arqsIndices = new Dictionary<string, ArquivoIndice>();
        }
        #endregion

        public void insertIndice(string indice, RegistroIndice registro, long posicao)
        {
            try {
                arqsIndices[indice].insert(registro, posicao);
            } catch (Exception e) {
                throw new SGDBException("Houve erro na inserção do indice! " + e.Message);
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
    }

    public sealed class Base
    {
        /*
        TODO:
        - Lista da classe de binarios [feito]
        - Acesso facilitado aos itens da lista 
        */
        private static Base instanciaUnica;
        public Dictionary<string, Binarios> arqBinarios;

        #region *** Construtores ***
        private Base()
        {
            arqBinarios = new Dictionary<string, Binarios>();
        }
        #endregion

        #region *** Getters e Setters ***
        public static Base getInstance()
        {
            if (instanciaUnica == null)
                instanciaUnica = new Base();

            return instanciaUnica;
        }
        #endregion

        public void insert(string tabela, RegistroTabela registro)
        {
            Binarios binAux = arqBinarios[tabela];
            long posicao = binAux.insertTabela(registro);
            insertIndices(binAux, registro, posicao, tabela);
        }

        private void insertIndices(Binarios binario, RegistroTabela registro, long posicao, string tabela)
        {
            
            RegistroIndice registroIndice;
            Metadados meta = GerenciadorMemoria.getInstance().recuperarMetadados(tabela);
            foreach (KeyValuePair<string, string[]> item in meta.getIndexes())
            {
                registroIndice = new RegistroIndice();
                for (int i = 0; i < item.Value.Length; i++)
                {
                    // ***** inibido somente para compilar - Inicio *****
                    // by Douglas Santos
                    // Acredito que isto va mudar
                    
                    DadoIndice dadoIndice = new DadoIndice(meta.getDados()[item.Value[i]].getTipoDado(), registro.dados[meta.getNomesColunas().IndexOf(item.Value[i])].valor);
                    registroIndice.dados.Add(dadoIndice);
                }
                // foi feito um registro agora é só adicionar
            }
        
        }

        public TabelaSelect returnDados(Metadados tabela)
        // Isto devera mudar para ser independente de 1 ou todos os registros
        {
            string arqTabela = GerenciadorMemoria.getInstance().getPath() + "\\" + tabela.getNome() + ".dat";
            return new ArquivoTabela(arqTabela).returnTudo(tabela.getNome(), arqTabela);
        }
    }
}
using System;

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
        private ArquivoIndice arqIndice { get; } // transformar em uma lista

        #region *** Construtores ***
        private Binarios(string pathTabela, string pathIndice)
        {
            this.memoria = new Memoria();
            this.arqTabela = new ArquivoTabela(pathTabela);
            this.arqIndice = new ArquivoIndice(pathIndice);
        }

        public Binarios(string pathTabela)
        {
            this.memoria = new Memoria();
            this.arqTabela = new ArquivoTabela(pathTabela);
            this.arqIndice = null;
        }
        #endregion

        public void insert(RegistroTabela registro)
        // Isto devera mudar para inserir em todos os indices da mesma tabela
        {
            try {
                long posicao = arqTabela.insert(registro);
                arqIndice.insert(registro, posicao);
            } catch (Exception e)
            {
                throw new SGDBException("Houve erro na inserção do registro! " + e.Message);
            }
        }

        public TabelaSelect returnDados(Metadados tabela)
        // Isto devera mudar para ser independente de 1 ou todos os registros
        {
            string arqTabela = GerenciadorMemoria.getInstance().getPath() + "\\" + tabela.getNome() + ".dat";
            return new ArquivoTabela(arqTabela).returnTudo(tabela.getNome(), arqTabela);
        }
    }

    public sealed class Base
    {
        /*
        TODO:
        - Lista da classe de binarios
        - Acesso facilitado aos itens da lista 
        */
    }
}

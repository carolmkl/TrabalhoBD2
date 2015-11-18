using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB.Dados
{
    /*
    Metodos necessarios para select
    tabelaSelect returnDados(String tabela) //select * from tabela;
    tabelaSelect returnDados(List<string> colunas, string tabela) //select tabela.campo1 from tabela;
    tabelaSelect returnDados(List<Filtro> filtro, List<string> colunas, string tabela) //select tabela.campo1 from tabela where tabela.campo1 = 0 AND ...;

    metodos necessario para Evandro
    salvarIndice(string[] valor, int lastPosi); - o nome já diz ele salva os dados com a posição no índice
    tabelaTemDados(string nomeTabela); - true se a tabela tem dados, false se não tiver
    int inserirDado(TabelaDado tabelaDado) - vai inserir o dado no arquivo da tabela;
    */
    class Base
    {
        private Memoria memoria { get; }
        private ArquivoTabela arqTabela { get; }
        private ArquivoIndice arqIndice { get; }

        public Base(string pathTabela, string pathIndice)
        {
            memoria = new Memoria();
            arqTabela = new ArquivoTabela(pathTabela);
            arqIndice = new ArquivoIndice(pathIndice);
        }

        public bool insert(Registro registro)
        {
            // Aqui vai inserir na tabela e no indice logo em seguida
            // qqr problema, false, erros geram exceções
            // true = fica tranquilo querido, tudo certo!
            return false;
        }
    }
}

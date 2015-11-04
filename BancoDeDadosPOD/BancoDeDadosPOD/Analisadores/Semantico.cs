using BancoDeDadosPOD.SGDB;
using BancoDeDadosPOD.SGDB.Select;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{ 

    public class Semantico : Constants
    {
        private enum acao: int { Nada=0, CriarTabela, InserirDados, Select, CriarIndex };
        private List<string> identificadores;
        private List<ValoresCampos> valoresColunas;
        private Dictionary<string,string> clausulaAs; //Carol: Sendo a cláusula AS usada apenas no select, é necessário este Dictionary?
        private Metadados metadados;// aqui é por hora, pode ser mudado para uma list por causa dos select, ou não
       
        /// <summary>
        /// Objeto utilizado para armazenar os dados do SELECT
        /// </summary>
        private Select select;

        /// <summary>
        /// Armazena os tokens de tabelas solicitados para comparar com os campos do SELECT
        /// </summary>
        private List<string> fromTabelas;

        /// <summary>
        /// indica se já acabou os joins
        /// </summary>
        private bool acabouJoin = false;

        // referente a ação semantica numero 6
        private bool sexta;

        // referente ao fato de usar todas as colunas ou não;
        private bool allColunas;
        // saber quantas colunas são pra ser adicionadas
        private int contColunas;

        // acho bom saber o que vai ser executado na ação 0 por isso dessa variavel, precisamos definir códigos pra ela
        private acao operacao;
        private  GerenciadorMemoria memoria;

        public Semantico()
        {
            identificadores = new List<string>();
            clausulaAs = new Dictionary<string, string>();
            valoresColunas = new List<ValoresCampos>();
            acaoZero();
            memoria = GerenciadorMemoria.getInstance();
            fromTabelas = new List<string>();
        }

        public void executeAction(int action, Token token) 
        {
            int index;
            switch (action)
            {
                case 0:
                    execucaoComandoReal();
                    acaoZero();
                    break;
                case 1:
                    memoria.createDatabase(token.getLexeme().ToLower());
                    break;
                case 2:
                    if (memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("TabelaSelect " + token.getLexeme().ToLower() + " já existe",  token.getPosition());
                    }
                    operacao =  acao.CriarTabela;
                    metadados.setNome(token.getLexeme().ToLower());
                    break;
                case 3:
                    if (memoria.existeIndex(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("Index " + token.getLexeme().ToLower() + " já existe", token.getPosition());
                    }
                    operacao = acao.CriarIndex;
                    identificadores.Add(token.getLexeme().ToLower());
                    break;
                case 4:
                    if (!memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("TabelaSelect " + token.getLexeme().ToLower() + " não existe", token.getPosition());
                    }
                    // Como é so saber o metadados, e ele tem o nome da tabela, não precisa ficar colocando a mesma no array
                    // de identificadores
                    metadados = GerenciadorMemoria.getInstance().recuperarMetadados(token.getLexeme().ToLower());
                    break;
                case 5:
                    identificadores.Add(token.getLexeme().ToLower());
                    break;
                case 6:
                    if (sexta)
                    {
                        for (int i = 0; i < identificadores.Count(); i++)
                        {
                            metadados.addDados(new DadosTabela(identificadores[i], valoresColunas[i].getTipo(), valoresColunas[i].getTamanho()));
                        }
                        identificadores.Clear();
                        valoresColunas.Clear();
                        sexta = false;
                    }
                    else
                    {
                        foreach (string id in identificadores)
                        {
                            if (!metadados.getDados().ContainsKey(id))
                            {
                                throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                            }
                            metadados.getDados()[id].setPrimary(true);
                        }
                        identificadores.Clear();
                    }
                    break;

                case 7:
                    if (!metadados.getDados().ContainsKey(token.getLexeme()))
                    {
                        throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                    }
                    identificadores.Add(token.getLexeme());
                    break;
                case 8:
                    if (!memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("TabelaSelect " + token.getLexeme().ToLower() + " não existe", token.getPosition());
                    }
                    identificadores.Add(token.getLexeme().ToLower());
                    break;
                case 9:
                    if (!memoria.recuperarMetadados(identificadores[1]).getDados().ContainsKey(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("Campo " + token.getLexeme().ToLower() + " na tabela " + identificadores[1] + "não existe", token.getPosition());
                    }
                    metadados.getDados()[identificadores[0]].setForeing(identificadores[1], token.getLexeme().ToLower());

                    // so pra salvar a alteração de mais uma chave estrangeira
                    Metadados aux = memoria.recuperarMetadados(identificadores[1]);
                    aux.getDados()[identificadores[1]].addForeing();
                    memoria.salvarMetadados(aux);

                    identificadores.Clear();
                    break;

                case 10:
                    // decidir tamanho
                    valoresColunas.Add(new ValoresCampos("INTEGER", 4));
                    break;

                case 11:
                    if (Convert.ToInt32(token.getLexeme()) > 255 || Convert.ToInt32(token.getLexeme()) < 1)
                    {
                        throw new SemanticError("Tamanho do campo " + identificadores.Last() + " inválido", token.getLinha());
                    }
                    valoresColunas.Add(new ValoresCampos("VARCHAR", Convert.ToInt32(token.getLexeme())));
                    break;

                case 12:
                    if (Convert.ToInt32(token.getLexeme()) > 255 || Convert.ToInt32(token.getLexeme()) < 1)
                    {
                        throw new SemanticError("Tamanho do campo " + identificadores.Last() +" inválido", token.getLinha());
                    }
                    valoresColunas.Add(new ValoresCampos("CHAR", Convert.ToInt32(token.getLexeme())));
                    break;
                case 13:
                    //Exclusão com verificação de foreing e exclusão das referencias
                    memoria.excluirTable(token.getLexeme().ToLower());
                    break;
                case 14:
                    //Exclusão de index
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 15:
                    // Fazer o resto em relação a isso
                    memoria.recuperarMetadados(token.getLexeme().ToLower()).ToString();
                    break;
                case 16:
                    memoria.setDatabase(token.getLexeme().ToLower());
                    break;
                case 17:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 18:
                    // Operador Relacional da cláusula Where do SELECT
                    if(select.Filtro == null)
                    {

                    }
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 19:
                    operacao = acao.InserirDados;
                    metadados = memoria.recuperarMetadados(identificadores[0]);
                    if (identificadores.Count() > 1)
                    {
                        allColunas = false;
                        contColunas = identificadores.Count() - 1;

                        for (int i = 1; i < identificadores.Count(); i++)
                        {
                            if (!metadados.getDados().ContainsKey(identificadores[i]))
                            {
                                throw new SemanticError("Campo " + identificadores[i] + "não existe na tabela " + identificadores[0], token.getLinha());
                            }
                        }
                    }
                    break;
                case 20:
                    if (!allColunas)
                    {
                        index = identificadores.Count() - 1 - contColunas;
                        if (index >= contColunas)
                        {
                            throw new SemanticError("Mais valores do que campos", token.getLinha());
                        }
                    }
                    else
                    {
                        index = identificadores.Count() - 1;
                        if (index >= metadados.getNomesColunas().Count())
                        {
                            throw new SemanticError("Mais valores do que campos", token.getLinha());
                        }
                        
                    }
                    if (ListaDeSimbolos.getInstance().classeToken(token.getId()).Contains(metadados.getDados()[metadados.getNomesColunas()[index]].geTipo()) || ListaDeSimbolos.getInstance().classeToken(token.getId()).Equals("null"))
                    {
                        // tipo tá correto
                        // validação de tamanho
                        if (ListaDeSimbolos.getInstance().classeToken(token.getId()).Equals("null"))
                        {
                            identificadores.Add(token.getLexeme());
                        }
                        else
                        {

                            if (token.getId() == 3)// Integer
                            {
                                if (Convert.ToUInt32(token.getLexeme()) <= UInt32.MaxValue)
                                {
                                    identificadores.Add(token.getLexeme());
                                }
                                else
                                {
                                    throw new SemanticError("Dado " + token.getLexeme() + " de tamanho incompativel" , token.getLinha());
                                }
                                
                            } else
                            {
                                if ((token.getLexeme().Length - 2) <= metadados.getDados()[metadados.getNomesColunas()[index]].getTamanho())
                                {
                                    identificadores.Add(token.getLexeme());
                                }
                                else
                                {
                                    throw new SemanticError("Dado " + token.getLexeme() + " de tamanho incompativel(" + metadados.getDados()[metadados.getNomesColunas()[index]].getTamanho() + ")", token.getLinha());
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        throw new SemanticError("Dado " + token.getLexeme() + " tem tipo incompativel com o campo " + metadados.getDados()[metadados.getNomesColunas()[index]] + " de tipo " + metadados.getDados()[metadados.getNomesColunas()[index]].geTipo(), token.getLinha());
                    }
                    break;
                case 21:
                    // tabela.campo --> Nome da coluna solicitada no SELECT
                    // Caso ainda não tenha sido definido, define a ação SELECT
                    select = Select.singleton();
                    operacao = acao.Select;
                    // verifica a existencia do campo
                    if (!memoria.recuperarMetadados(identificadores.Last()).getNomesColunas().Exists(s => s.Equals(token.getLexeme())))  
                    {
                        throw new SemanticError("Campo " + identificadores.Last()+ "." + token.getLexeme() + " não existe", token.getLinha());
                    }
                    // Adiciona o campo de retorno do SELECT
                    select.addTabela(identificadores.Last());
                    identificadores[identificadores.Count()-1] = identificadores.Last() + "." + token.getLexeme().ToLower();
                    select.addRetorno(identificadores.Last());
                    //Carol: Tem necessidade de incluir o token no identificadores, sendo que existe o objeto Select?
                    break;
                case 22:
                    //token do apelido da cláusula AS. O SELECT adiciona o apelido no último campo adicionado.
                    clausulaAs[identificadores.Last()] = token.getLexeme(); //Carol: esta linha é necessária?
                    select.addApelidoUltimo(token.getLexeme());
                    break;
                case 23:
                    //tabela.* --> selecão de todos os campos de uma tabela
                    //Caso ainda não tenha sido definido, define a ação SELECT
                    select = Select.singleton();
                    operacao = acao.Select;
                    //busca a tabela armazenada na última ação semântica
                    string tabela = identificadores.Last();
                    identificadores.RemoveAt(identificadores.Count() - 1);
                    //inclui a tabela no objeto SELECT
                    select.addTabela(tabela);
                    //busca as colunas da tabela para incluir no retorno
                    foreach(String col in memoria.recuperarMetadados(tabela).getNomesColunas())
                    {
                        string coluna = tabela + "." + col;
                        identificadores.Add(coluna); //Carol: estou inserindo no identificadores também porque ainda não sei se isto será usado em outro momento
                        select.addRetorno(coluna);
                    }
                    break;
                case 24:
                    // FROM tabelas
                    // inclui as tabelas numa lista a parte para validar depois com a classe select.
                    fromTabelas.Add(token.getLexeme());
                    break;
                case 25:
                    throw new SemanticError("Ação INNER JOIN não suportada.");
                case 26:
                    throw new SemanticError("Ação LEFT JOIN não suportada.");
                case 27:
                    throw new SemanticError("Ação RIGHT JOIN não suportada.");
                case 28:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 29:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 30:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
            }
            Console.WriteLine("Ação #" + action + ", Token: " + token);
        }

        private void execucaoComandoReal()
        {
            string id;
            switch (operacao)
            {
                case acao.Nada:
                    //throw new SGDBException("Que ação é essa? Favor incluir um comando válido.");
                    break;
                case acao.CriarTabela:
                    // metadados.criarIndiciePrimary()
                    memoria.salvarMetadados(metadados);
                    break;

                case acao.InserirDados:
                    TabelaSelect t = new TabelaSelect();
                    id = identificadores[0];
                    identificadores.RemoveAt(0);

                    t.Campos = metadados.getNomesColunas().ToArray();
                    if (allColunas)
                    {
                        t.addRegistro(identificadores.ToArray());
                    }
                    else
                    {
                        bool nacho = true;
                        string[] dados = new string[metadados.getNomesColunas().Count()];
                        for (int i = 0; i < dados.Length; i++)
                        {
                            nacho = true;
                            for (int j = 0; j < contColunas && nacho; j++)
                            {
                                if (metadados.getNomesColunas()[i].Equals(identificadores[j]))
                                {
                                    dados[i] = identificadores[j+contColunas];
                                    nacho = false;
                                }
                            }
                            if (nacho)
                            {
                                dados[i] = "null";
                            }
                        }
                        t.addRegistro(dados);
                    }
                    Console.WriteLine("TO STRING DA TABELA");
                    Console.WriteLine(t.ToString());


                    //inserir dados no arquivo
                    //metadados.addIncice(t, posi);
                    // memoria.salvar(metadados)
                    break;
                case acao.Select:
                    break;
                case acao.CriarIndex:
                    id = identificadores[0];
                    identificadores.RemoveAt(0);
                    foreach (string item in identificadores)
                    {
                        if (!metadados.getDados().ContainsKey(item))
                        {
                            new SemanticError("A coluna " + item + "não existe na tabela " + metadados.getNome());
                        }
                    }
                    if (metadados.getIndexes().ContainsKey(id))
                    {
                        new SemanticError("O indice " + id + "já existe na tabela " + metadados.getNome());
                    }
                    // Criar o index
                    //metadados.criarIndex()
                    //memoria.salvar(metadados)
                    break;
                default:
                    throw new SGDBException("Ação Real" + operacao + " não implementada.");
                    break;
            }
            
        }

        private void acaoZero()
        {
            identificadores.Clear();
            clausulaAs.Clear();
            fromTabelas.Clear();
            valoresColunas.Clear();
            metadados = new Metadados();
            sexta = true;
            allColunas = true;
            contColunas = 0;
            operacao = 0;
        }
    }
}

using BancoDeDadosPOD.SGDB;
using BancoDeDadosPOD.SGDB.Select;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BancoDeDadosPOD;
using BancoDeDadosPOD.SGDB.Dados;

namespace BD2.Analizadores
{

    public class Semantico : Constants
    {
        private enum acao : int { Nada = 0, CriarTabela, InserirDados, Select, CriarIndex, ExcluirIndex };
        private List<string> identificadores;
        private List<ValoresCampos> valoresColunas;
        //private Dictionary<string, string> clausulaAs; //Carol: Sendo a cláusula AS usada apenas no select, é necessário este Dictionary?
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
        /// armazena o último filtro que está sendo analisado pelas ações semanticas
        /// </summary>
        private Filtro ultimoFiltro;

        // referente a ação semantica numero 6
        private bool sexta;

        // referente ao fato de usar todas as colunas ou não;
        private bool allColunas;
        // saber quantas colunas são pra ser adicionadas
        private int contColunas;

        // acho bom saber o que vai ser executado na ação 0 por isso dessa variavel, precisamos definir códigos pra ela
        private acao operacao;
        private GerenciadorMemoria memoria;
        private Form1 form1;

        public Semantico()
        {
            identificadores = new List<string>();
            //clausulaAs = new Dictionary<string, string>();
            valoresColunas = new List<ValoresCampos>();
            fromTabelas = new List<string>();

            acaoZero();
            memoria = GerenciadorMemoria.getInstance();
        }

        public Semantico(Form1 form1)
        {
            this.form1 = form1;
            identificadores = new List<string>();
            //clausulaAs = new Dictionary<string, string>();
            valoresColunas = new List<ValoresCampos>();
            fromTabelas = new List<string>();

            acaoZero();
            memoria = GerenciadorMemoria.getInstance();
        }

        public void executeAction(int action, Token token)
        {
            int index;
            if((action != 16 && action !=1) && memoria.getDatabase() == null)
            {
                throw new SGDBException("utilize o comando SET DATABASE...");
            }
            switch (action)
            {
                case 0:
                    execucaoComandoReal();
                    acaoZero();
                    break;
                case 1:
                    memoria.createDatabase(token.getLexeme().ToLower());
                    form1.Text = memoria.getDatabase();
                    break;
                case 2:
                    if (memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " já existe", token.getLinha());
                    }
                    operacao = acao.CriarTabela;
                    metadados.setNome(token.getLexeme().ToLower());
                    break;
                case 3:
                    if (memoria.existeIndex(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("Index " + token.getLexeme().ToLower() + " já existe", token.getLinha());
                    }
                    operacao = acao.CriarIndex;
                    identificadores.Add(token.getLexeme().ToLower());
                    break;
                case 4:
                    if (!memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " não existe", token.getPosition());
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
                        Console.WriteLine(identificadores.Count);
                        Console.WriteLine(valoresColunas.Count);
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
                        throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " não existe", token.getPosition());
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
                        throw new SemanticError("Tamanho do campo " + identificadores.Last() + " inválido", token.getLinha());
                    }
                    valoresColunas.Add(new ValoresCampos("CHAR", Convert.ToInt32(token.getLexeme())));
                    break;
                case 13:
                    //Exclusão com verificação de foreing e exclusão das referencias
                    memoria.excluirTable(token.getLexeme().ToLower());
                    break;
                case 14:
                    //Exclusão de index
                    if (memoria.existeIndex(token.getLexeme().ToLower()))
                    {
                        identificadores.Add(token.getLexeme().ToLower());
                        operacao = acao.ExcluirIndex;   
                    }
                    else
                    {
                        throw new SemanticError("Index " + token.getLexeme().ToLower() + "não existe", token.getLinha());
                    }
                    break;
                case 15:
                    // Fazer o resto em relação a isso
                    memoria.recuperarMetadados(token.getLexeme().ToLower()).ToString();
                    break;
                case 16:
                    memoria.setDatabase(token.getLexeme().ToLower());
                    form1.Text = memoria.getDatabase();
                    break;
                case 17:
                    if (ultimoFiltro != null)
                        throw new SGDBException("Falha na operação semântica 17. O Filtro deveria estar nulo.");
                    ultimoFiltro = new Filtro();
                    if (token.getId() == 16) ultimoFiltro.IsAND = true;
                    else if (token.getId() == 17) ultimoFiltro.IsOR = true;
                    break;
                case 18:
                    // Operador Relacional da cláusula Where do SELECT
                    select.Where = select.Where == null ? new Where() : select.Where;
                    ultimoFiltro = ultimoFiltro == null ? new Filtro() : ultimoFiltro;
                    //Joga o campo para o LValue, caso tenha sido incluído como retorno do select. Acontece no primeiro filtro.
                    //ultimoFiltro.LValue = ultimoFiltro.LValue == null? select.removeUltimoRetorno(): ultimoFiltro.LValue;
                    switch (token.getId())
                    {
                        case 37:
                            ultimoFiltro.Op = OperadorRel.Igual;
                            break;
                        case 38:
                            ultimoFiltro.Op = OperadorRel.MaiorQue;
                            select.Etapa = Select.EtapaSemantica.WHERE;
                            break;
                        case 39:
                            ultimoFiltro.Op = OperadorRel.MenorQue;
                            select.Etapa = Select.EtapaSemantica.WHERE;
                            break;
                        case 40:
                            ultimoFiltro.Op = OperadorRel.MaiorIgualA;
                            select.Etapa = Select.EtapaSemantica.WHERE;
                            break;
                        case 41:
                            ultimoFiltro.Op = OperadorRel.MenorIgualA;
                            select.Etapa = Select.EtapaSemantica.WHERE;
                            break;
                        case 42:
                            ultimoFiltro.Op = OperadorRel.Diferente;
                            select.Etapa = Select.EtapaSemantica.WHERE;
                            break;
                        default:
                            break;
                    }
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
                                    throw new SemanticError("Dado " + token.getLexeme() + " de tamanho incompativel", token.getLinha());
                                }

                            }
                            else
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
                        throw new SemanticError("Campo " + identificadores.Last() + "." + token.getLexeme() + " não existe", token.getLinha());
                    }
                    switch (select.Etapa)
                    {
                        case Select.EtapaSemantica.CAMPOS:
                            // Adiciona o campo de retorno do SELECT
                            select.addTabela(identificadores.Last());
                            string ret = identificadores.Last() + "." + token.getLexeme().ToLower();
                            identificadores.Remove(identificadores.Last());
                            select.addRetorno(ret);
                            break;
                        case Select.EtapaSemantica.TABELA:
                            throw new SGDBException("Não devia passar por aqui. Ação semantica 21. Select.ETAPA = Tabela");
                            
                        case Select.EtapaSemantica.JOIN:
                            ultimoFiltro = ultimoFiltro == null ? new Filtro() : ultimoFiltro; //necessário na primeira linha do where
                            if (ultimoFiltro.LValue == null)
                            {
                                //se ainda não foi atribuído LValue, então deve estar no lado esquerdo da operação.
                                ultimoFiltro.LValue = identificadores.Last() + "." + token.getLexeme().ToLower();
                                identificadores.Remove(identificadores.Last());
                            }
                            else
                            {

                                if (select.Etapa == Select.EtapaSemantica.WHERE)
                                {
                                    throw new SemanticError("Os primeiros filtros devem ser de JOIN. Cláusula JOIN já finalizada.", token.getLinha());
                                }
                                if (ultimoFiltro.IsOR)
                                {
                                    throw new SemanticError("JOIN deve utilizar AND", token.getLinha());
                                }
                                ultimoFiltro.RValue = identificadores.Last() + "." + token.getLexeme();
                                identificadores.Remove(identificadores.Last());
                                ultimoFiltro.IsAND = true;
                                select.Where.addJoin(ultimoFiltro);
                                ultimoFiltro = null;
                            }
                            break;
                        case Select.EtapaSemantica.WHERE:
                            ultimoFiltro.LValue = identificadores.Last() + "." + token.getLexeme().ToLower();
                            identificadores.Remove(identificadores.Last());
                            break;
                        case Select.EtapaSemantica.ORDER:
                            string campo = identificadores.Last() + "." + token.getLexeme().ToLower();
                            select.addOrderBy(campo);
                            identificadores.Remove(identificadores.Last());
                            break;
                        default:
                            throw new SGDBException("Não devia passar por aqui. Ação semantica 21. Select.ETAPA = Default");
                    }

                    break;
                case 22:
                    //token do apelido da cláusula AS. O SELECT adiciona o apelido no último campo adicionado.
                    //clausulaAs[identificadores.Last()] = token.getLexeme(); //Carol: esta linha é necessária?
                    if(select.Where != null)
                    {
                        //se já passou pelo where, provavelmente está no ORDER BY
                        throw new SemanticError("Não existe AS pra ORDER BY!", token.getLinha());
                    }
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

                    foreach (String col in memoria.recuperarMetadados(tabela).getNomesColunas())
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
                    throw new SemanticError("Ação INNER JOIN não suportada neste formato. Insira um filtro no Where.");
                case 26:
                    throw new SemanticError("Ação LEFT JOIN não suportada.");
                case 27:
                    throw new SemanticError("Ação RIGHT JOIN não suportada.");
                case 28:
                    //final do ORDER BY. Verificar se os campos constam no retorno.
                    //Não necessário, pois é verificado ao inserir cada campo, na ação 21.
                    break;
                case 29:
                    //Define a ordenação dos campos como descrecente
                    select.orderDesc();
                    break;
                case 30:
                    select.Etapa = Select.EtapaSemantica.WHERE;
                    ultimoFiltro.RValue = token.getLexeme();
                    //ultimoFiltro.IsOR? select.Where:break;
                    if (ultimoFiltro.IsOR) select.Where.addFiltroOR(ultimoFiltro);
                    else select.Where.addFiltroAND(ultimoFiltro);
                    ultimoFiltro = null;
                    break;

                case 31:
                    //logo depois do FROM
                    select.Etapa = Select.EtapaSemantica.TABELA;
                    break;
                case 32:
                    //logo antes do Where. o where inicia com possível join.
                    select.Etapa = Select.EtapaSemantica.JOIN;
                    break;
                case 33:
                    //logo antes do order
                    select.Etapa = Select.EtapaSemantica.ORDER;
                    break;
                default:
                    throw new SGDBException("Ação " + action + " não implementada.");
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
                    metadados.criarIndiciePrimary();
                    memoria.salvarMetadados(metadados);
                    memoria.recuperarMetadados();
                    break;

                case acao.InserirDados:
                    
                    id = identificadores[0];
                    identificadores.RemoveAt(0);
                    TabelaDado t = new TabelaDado(id, memoria.getDiretorioPath());

                    /*t.Campos = metadados.getNomesColunas().ToArray();
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
                                    dados[i] = identificadores[j + contColunas];
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
                    */

                    //inserir dados no arquivo
                    //metadados.addIncice(t, posi);
                    // memoria.salvar(metadados)
                    break;
                case acao.Select:
                    Form1.addMensagem(select.ToString());

                    select.clear();
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
                    // Garantir nome unico de index
                    //metadados.criarIndex()
                    //memoria.salvar(metadados)
                    break;

                case acao.ExcluirIndex:
                    memoria.excluirIndex(identificadores[0]);
                    memoria.salvarMetadados();
                    memoria.atualizar();
                    break;
                default:
                    throw new SGDBException("Ação Real" + operacao + " não implementada.");
            }
            Console.WriteLine(metadados.StringIndices());
        }

        private void acaoZero()
        {
            identificadores.Clear();
            //clausulaAs.Clear();
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

using BancoDeDadosPOD.SGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    public class Semantico : Constants
    {
        private List<string> identificadores;
        private List<ValoresCampos> valoresColunas;
        private Dictionary<string,string> clausulaAs;
        private Metadados metadados;// aqui é por hora, pode ser mudado para uma list por causa dos select, ou não

        // referente a ação semantica numero 6
        private bool sexta;

        // referente ao fato de usar todas as colunas ou não;
        private bool allColunas;
        // saber quantas colunas são pra ser adicionadas
        private int contColunas;

        // acho bom saber o que vai ser executado na ação 0 por isso dessa variavel, precisamos definir códigos pra ela
        private int operacao;
        private  GerenciadorMemoria memoria;

        public Semantico()
        {
            identificadores = new List<string>();
            clausulaAs = new Dictionary<string, string>();
            valoresColunas = new List<ValoresCampos>();
            metadados = new Metadados();
            sexta = true;
            allColunas = true;
            contColunas = 0;
            memoria = GerenciadorMemoria.getInstance();
            operacao = 0;
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
                        acaoZero();
                        throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " já existe",  token.getPosition());
                    }
                    operacao = 1;
                    metadados.setNome(token.getLexeme().ToLower());
                    break;
                case 3:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 4:
                    throw new SGDBException("Ação " + action + " não implementada.");
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
                                acaoZero();
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
                        acaoZero();
                        throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                    }
                    identificadores.Add(token.getLexeme());
                    break;
                case 8:
                    if (!memoria.existeTabela(token.getLexeme().ToLower()))
                    {
                        acaoZero();
                        throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " não existe", token.getPosition());
                    }
                    identificadores.Add(token.getLexeme().ToLower());
                    break;
                case 9:
                    if (!memoria.recuperarMetadados(identificadores[1]).getDados().ContainsKey(token.getLexeme().ToLower()))
                    {
                        acaoZero();
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
                        acaoZero();
                        throw new SemanticError("Tamanho do campo " + identificadores.Last() + " inválido", token.getLinha());
                    }
                    valoresColunas.Add(new ValoresCampos("VARCHAR", Convert.ToInt32(token.getLexeme())));
                    break;

                case 12:
                    if (Convert.ToInt32(token.getLexeme()) > 255 || Convert.ToInt32(token.getLexeme()) < 1)
                    {
                        acaoZero();
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
                    memoria.setSubPastaPath(token.getLexeme().ToLower());
                    break;
                case 17:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 18:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 19:
                    // 2 é o insert
                    operacao = 2;
                    if (identificadores.Count() > 1)
                    {
                        allColunas = false;
                        contColunas = identificadores.Count() - 1;
                    }
                    break;
                case 20:
                    index = identificadores.Count();
                    if (!allColunas)
                    {
                        index =  1 - contColunas;

                        metadados = memoria.recuperarMetadados(identificadores[0]);
                        if (index > contColunas)
                        {
                            throw new SemanticError("Mais valores do que campos", token.getLinha());
                        }
                    }
                    else
                    {
                        index = identificadores.Count() - 1;
                        metadados = memoria.recuperarMetadados(identificadores[0]);
                        if (index > metadados.getNomesColunas().Count())
                        {
                            throw new SemanticError("Mais valores do que campos", token.getLinha());
                        }
                        
                    }
                    if (metadados.getDados()[metadados.getNomesColunas()[index]].geTipo().Equals(ListaDeSimbolos.getInstance().classeToken(token.getId())) || ListaDeSimbolos.getInstance().classeToken(token.getId()).Equals("null"))
                    {
                        identificadores.Add(token.getLexeme());
                    }
                    else
                    {
                        throw new SemanticError("Dado " + token.getLexeme() + "tem tipo incompativel com o campo " + metadados.getDados()[metadados.getNomesColunas()[index]].geTipo(), token.getLinha());
                    }
                    break;
                case 21:
                    //esboço
                    if (!metadados.getDados().ContainsKey(token.getLexeme()))
                    {
                        acaoZero();
                        throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                    }
                    identificadores[identificadores.Count()] = identificadores.Last() + "." + token.getLexeme().ToLower();
                    break;
                case 22:
                    clausulaAs[identificadores.Last()] = token.getLexeme();
                    break;
                case 23:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 24:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 25:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 26:
                    throw new SGDBException("Ação " + action + " não suportada.");
                    break;
                case 27:
                    throw new SGDBException("Ação " + action + " não suportada.");
                    break;
                case 28:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 29:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
            }
            Console.WriteLine("Ação #" + action + ", Token: " + token);
        }

        private void execucaoComandoReal()
        {
            switch (operacao)
            {
                case 0:
                    break;
                case 1:
                    memoria.salvarMetadados(metadados);
                    break;

                case 2:
                    memoria.salvarMetadados(metadados);
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
            valoresColunas.Clear();
            metadados = new Metadados();
            sexta = true;
            allColunas = true;
            contColunas = 0;
            operacao = 0;
        }
    }
}

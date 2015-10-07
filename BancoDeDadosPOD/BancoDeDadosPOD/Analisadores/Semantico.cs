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
        private Dictionary<string,string> clausulaAs;
        private Metadados metadados;// aqui é por hora, pode ser mudado para uma list por causa dos select, ou não
        private bool sexta;
        // acho bom saber o que vai ser executado na ação 0 por isso dessa variavel, precisamos definir códigos pra ela
        private int operacao;
        private  GerenciadorMemoria memoria;

        public Semantico()
        {
            identificadores = new List<string>();
            clausulaAs = new Dictionary<string, string>();
            metadados = new Metadados();
            sexta = true;
            memoria = new GerenciadorMemoria();
            operacao = 0;
        }

        public void executeAction(int action, Token token) 
        {
            switch (action)
            {
                case 0:
                    // ações referentes a execuções
                    identificadores.Clear();
                    clausulaAs.Clear();
                    metadados = new Metadados();
                    sexta = true;
                    operacao = 0;
                    break;
                case 1:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 2:
                    //if (!jaexiste)
                    //{
                     //   throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " já existe",  token.getPosition());
                    //}
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
                        // adiciona o dado no meta;
                        identificadores.Clear();
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
                    // verifica se tabela existe
                    identificadores.Add(token.getLexeme());
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 9:
                    // verifica se o campo existe na outra tabela
                    metadados.getDados()[identificadores[0]].setForeing(identificadores[1], identificadores[2]);
                    identificadores.Clear();
                    break;
                case 10:
                    // decidir tamanho
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 11:
                    //decidir tamanho
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 12:
                    //decidir tamanho
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 13:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 14:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 15:
                    //memoria.show(token.getLexeme());
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 16:
                    // memoria.algo(token.getLexeme());
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 17:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 18:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 19:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 20:
                    //esboço
                    if (!metadados.getDados().ContainsKey(token.getLexeme()))
                    {
                        throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                    }
                    identificadores[identificadores.Count()] = identificadores.Last() + "." + token.getLexeme().ToLower();
                    break;
                case 21:
                    clausulaAs[identificadores.Last()] = token.getLexeme();
                    break;
                case 22:
                    throw new SGDBException("Ação " + action + " não implementada.");
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
                case 28:
                    throw new SGDBException("Ação " + action + " não suportada.");
                    break;
                case 29:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
            }
            Console.WriteLine("Ação #" + action + ", Token: " + token);
        }
    }
}

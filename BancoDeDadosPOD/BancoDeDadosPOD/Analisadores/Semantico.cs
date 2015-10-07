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
        private Metadados metadados;// aqui é por hora, pode ser mudado para uma list por causa dos select, ou não
        private bool sexta;
        // acho bom saber o que vai ser executado na ação 0 por isso dessa variavel, precisamos definir códigos pra ela
        private int operacao;

        public Semantico()
        {
            identificadores = new List<string>();
            metadados = new Metadados();
            sexta = true;
        }

        public void executeAction(int action, Token token) 
        {
            switch (action)
            {
                case 0:
                    identificadores.Clear();
                    metadados = new Metadados();
                    sexta = true;
                    throw new SGDBException("Ação "+ action + " não implementada.");
                    break;
                case 1:
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 2:
                    //if (!jaexiste)
                    //{
                     //   throw new SemanticError("Tabela " + token.getLexeme().ToLower() + " já existe",  token.getPosition());
                    //}
                    //else
                    //{
                        metadados.setNome(token.getLexeme().ToLower());
                    //}

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
                            if (metadados.getDados().ContainsKey(id))
                            {
                                metadados.getDados()[id].setPrimary(true);
                            }
                            else
                            {
                                throw new SemanticError("Campo " + token.getLexeme() + " não existe", token.getLinha());
                            }
                        }
                        identificadores.Clear();
                    }
                    break;

                case 7:
                    if (metadados.getDados().ContainsKey(token.getLexeme()))
                    {
                        identificadores.Add(token.getLexeme());
                    }
                    else
                    {
                        throw new SemanticError("Campo " + token.getLexeme() + " não existe",token.getLinha());
                    }
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
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 16:
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
                    throw new SGDBException("Ação " + action + " não implementada.");
                    break;
                case 21:
                    throw new SGDBException("Ação " + action + " não implementada.");
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

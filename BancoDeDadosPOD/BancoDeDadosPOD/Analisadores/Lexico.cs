using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    public class Lexico : Constants
    {
        private int position;
        private String input;
        private int linha;
        private int linhaInterna;
        private char vNextChar;

        public Lexico()
        {
            setInput("");
        }

        public Lexico(String input)
        {
            setInput(input);
        }

        public void setInput(String input)
        {
            this.input = input;
            setPosition(0);
        }

        public void setPosition(int pos)
        {
            position = pos;
        }

        public Token nextToken()
        {
            if (!hasInput())
            {
                return null;
            }
            int start = position;

            int state = 0;
            int lastState = 0;
            int endState = -1;
            int end = -1;

            while (hasInput())
            {
                lastState = state;
                vNextChar = nextChar();
                state = nextState(vNextChar, state);

                if (state < 0)
                    break;

                else
                {
                    if (tokenForState(state) >= 0)
                    {
                        endState = state;
                        end = position;
                    }
                }
                if (vNextChar == '\n')
                {
                    linha++;
                }
            }
            if (endState < 0 || (endState != state && tokenForState(lastState) == -2))
            {
                Console.WriteLine(input.Substring(start, position - start));
                throw new LexicalError(SCANNER_ERROR[lastState], linhaInterna);
            }

            position = end;

            int token = tokenForState(endState);
            linhaInterna = linha;

            if (token == 0)
            {
                return nextToken();
            }
            else
            {
                String lexeme = input.Substring(start, end - start);
                token = lookupToken(token, lexeme);
                return new Token(token, lexeme, start, linha);
            }
        }

        private int nextState(char c, int state)
        {
            int next = SCANNER_TABLE[state, c];
            return next;
        }

        private int tokenForState(int state)
        {
            if (state < 0 || state >= TOKEN_STATE.Count())
            {
                return -1;
            }
            return TOKEN_STATE[state];
        }

        public int lookupToken(int bas, String key)
        {
            int start = SPECIAL_CASES_INDEXES[bas];
            int end = SPECIAL_CASES_INDEXES[bas + 1] - 1;

            key = key.ToUpper();

            while (start <= end)
            {
                int half = (start + end) / 2;
                int comp = SPECIAL_CASES_KEYS[half].CompareTo(key);

                if (comp == 0)
                {
                    return SPECIAL_CASES_VALUES[half];
                }
                else if (comp < 0)
                {
                    start = half + 1;
                }
                else
                { //(comp > 0)
                    end = half - 1;
                }
            }

            return bas;
        }

        private bool hasInput()
        {
            return position < input.Count();
        }

        private char nextChar()
        {
            if (hasInput())
            {
                return input[position++];
            }
            else
            {
                return Convert.ToChar(-1);
            }
        }
    }
}

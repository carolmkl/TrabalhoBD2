using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD2.Analizadores
{
    public class Constants : ParserConstants
    {
        public int EPSILON = 0;
        public int DOLLAR = 1;

        public int t_id = 2;
        public int t_numero = 3;
        public int t_literal = 4;
        public int t_CREATE = 5;
        public int t_DATABASE = 6;
        public int t_TABLE = 7;
        public int t_INDEX = 8;
        public int t_ON = 9;
        public int t_INSERT = 10;
        public int t_INTO = 11;
        public int t_VALUES = 12;
        public int t_SELECT = 13;
        public int t_FROM = 14;
        public int t_WHERE = 15;
        public int t_AND = 16;
        public int t_OR = 17;
        public int t_DROP = 18;
        public int t_DESCRIBE = 19;
        public int t_SET = 20;
        public int t_ORDER = 21;
        public int t_BY = 22;
        public int t_DESC = 23;
        public int t_AS = 24;
        public int t_PRIMARY = 25;
        public int t_FOREIGN = 26;
        public int t_KEY = 27;
        public int t_REFERENCES = 28;
        public int t_JOIN = 29;
        public int t_INNER = 30;
        public int t_LEFT = 31;
        public int t_RIGHT = 32;
        public int t_integer = 33;
        public int t_varchar = 34;
        public int t_char = 35;
        public int t_null = 36;
        public int t_TOKEN_37 = 37; //"="
        public int t_TOKEN_38 = 38; //">"
        public int t_TOKEN_39 = 39; //"<"
        public int t_TOKEN_40 = 40; //">="
        public int t_TOKEN_41 = 41; //"<="
        public int t_TOKEN_42 = 42; //"<>"
        public int t_TOKEN_43 = 43; //"."
        public int t_TOKEN_44 = 44; //","
        public int t_TOKEN_45 = 45; //"*"
        public int t_TOKEN_46 = 46; //";"
        public int t_TOKEN_47 = 47; //"`"
        public int t_TOKEN_48 = 48; //"("
        public int t_TOKEN_49 = 49; //")"
    }
}

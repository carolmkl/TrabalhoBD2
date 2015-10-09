using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDeDadosPOD.SGDB
{
    public class ValoresCampos
    {
        private string tipo;
        private int tamanho;
        private string v1;
        private string v2;

        public ValoresCampos() { }

        public ValoresCampos(string tipo, int tamanho)
        {
            setTipo(tipo);
            setTamanho(tamanho);
        }

        public ValoresCampos(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public void setTipo(string tipo)
        {
            this.tipo = tipo;
        }
        public void setTamanho(int tamanho)
        {
            this.tamanho = tamanho;
        }

        public string getTipo()
        {
            return tipo;
        }
        public int getTamanho()
        {
            return tamanho;
        }
    }
}

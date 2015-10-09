using BD2.Analizadores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BancoDeDadosPOD
{
    public partial class Form1 : Form
    {

        private static TextBox mensagens;
        public Form1()
        {
            InitializeComponent();
            mensagens = txtMensagens;
            this.KeyUp += new KeyEventHandler(f5);
            this.txtComando.KeyUp += new KeyEventHandler(f5);
        }

        private void f5(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F5)
            {
                executa();
            }
        }

        private void btnExecuta_Click(object sender, EventArgs e)
        {
            executa();
        }

        public void executa()
        {
            try
            {
                clearMensagem();
                Lexico lexico = new Lexico(txtComando.Text);
                Sintatico sintatico = new Sintatico();
                Semantico semantico = new Semantico();
                sintatico.parse(lexico, semantico);
                addMensagem("Success!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                addMensagem("#ERROR: " + ex.ToString());
            }
        }

        public static void addMensagem(String txt)
        {
            mensagens.Text += txt + " \r\n";
            mensagens.Refresh();
        }

        public static void clearMensagem()
        {
            mensagens.Clear();
            mensagens.Refresh();
        }
    }
}

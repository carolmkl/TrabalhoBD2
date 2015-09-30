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
        public Form1()
        {
            InitializeComponent();
        }

        private void btnExecuta_Click(object sender, EventArgs e)
        {
            try
            {
                Lexico lexico = new Lexico(txtComando.Text);
                Sintatico sintatico = new Sintatico();
                Semantico semantico = new Semantico();
                sintatico.parse(lexico, semantico);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}

using BancoDeDadosPOD.SGDB;
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
        private static DataGridView gridView;
        public Form1()
        {
            InitializeComponent();
            mensagens = txtMensagens;
            gridView = dataGridView1;
            this.KeyUp += new KeyEventHandler(f5);
            this.txtComando.KeyUp += new KeyEventHandler(f5);
        }

        private void f5(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
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
                tabResultado.SelectedIndex = 0;
                clearMensagem();
                addMensagem("Executando...");
                Lexico lexico = new Lexico(txtComando.Text);
                Sintatico sintatico = new Sintatico();
                Semantico semantico = new Semantico(this);
                sintatico.parse(lexico, semantico);
                addMensagem("Success!!!");
            }
            catch (Exception ex)
            {
                addMensagem("#ERROR: " + ex.ToString());
                //Console.WriteLine(ex.StackTrace);
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

        public static void setResultado(TabelaSelect tabela, Dictionary<string, string> retorno)
        {
            gridView.Columns.Clear();
            int[] indices = new int[retorno.Count];
            for (int i = 0; i < retorno.Count; i++)
            {
                KeyValuePair<string, string> campo = retorno.ElementAt(i);
                //insere a coluna no gridView com o apelido definido
                gridView.Columns.Add(campo.Key, campo.Value);
                //verifica qual eh o índice da tabelaSelect correspondente ao campo para ordenar depois
                for (int j = 0; j < tabela.Campos.Length; j++)
                {
                    if (tabela.Campos[j].Equals(campo.Key))
                    {
                        indices[i] = j;
                        break;
                    }
                }
            }
            foreach (string[] registro in tabela.Registros)
            {
                string[] linha = new string[registro.Length];
                for (int i = 0; i < linha.Length; i++)
                {
                    linha[i] = registro[indices[i]];
                }
                gridView.Rows.Add(linha);
            }
        }

        public static void setResultado(Metadados meta)
        {
            gridView.Columns.Clear();
            string[] head = new string[] { "Campo", "Tipo", "Tamanho", "Primary", "Foreign" };
            foreach (string s in head)
            {
                gridView.Columns.Add(s, s);
            }
            foreach (DadosTabela d in meta.getDados().Values)
            {
                gridView.Rows.Add(d.getNomeCampo(), d.geTipo(), d.getTamanho(), d.isPrimary(), (d.isForeing() ? d.getForeing()[0] + "(" + d.getForeing()[1] + ")" : "False"));
            }
            gridView.Refresh();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            tabResultado.SelectedIndex = 1;
        }
    }
}

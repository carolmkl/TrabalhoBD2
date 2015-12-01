using BancoDeDadosPOD.SGDB;
using BancoDeDadosPOD.SGDB.Dados;
using BD2.Analizadores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        OpenFileDialog ofd;

        public Form1()
        {
            InitializeComponent();
            mensagens = txtMensagens;
            gridView = dataGridView1;
            //gridView.RowTemplate.
            this.KeyUp += new KeyEventHandler(f5);
            this.txtComando.KeyUp += new KeyEventHandler(f5);
            ofd = new OpenFileDialog();
            ofd.Filter = "(Arquivos SQL)|*.sql";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.RestoreDirectory = true;
        }

        private void f5(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                executa(txtComando.Text);
            }
        }

        private void btnExecuta_Click(object sender, EventArgs e)
        {
            executa(txtComando.Text);
        }

        public void executa(string comandos)
        {
            try
            {
                tabResultado.SelectedIndex = 0;
                clearMensagem();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                addMensagem("Executando...");
                Semantico semantico = new Semantico(this);
                Lexico lexico = new Lexico(comandos);
                Sintatico sintatico = new Sintatico();
                sintatico.parse(lexico, semantico);
                semantico.Dispose();
                semantico = null;
                sw.Stop();

                Base.getInstance().commit();

                TimeSpan tempo = sw.Elapsed;
                addMensagem(String.Format("Sucesso!!! Tempo de Execução: {0}min {1}s {2}ms", tempo.Minutes, tempo.Seconds, tempo.Milliseconds));
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

            if (tabela == null)
            {
                addMensagem(String.Format("{0} linhas selecionadas.", 0));
                return;
            }

            addMensagem(String.Format("{0} linhas selecionadas.", tabela.Registros.Count));
            int[] indices = new int[retorno.Count];
            for (int i = 0; i < retorno.Count; i++)
            {
                KeyValuePair<string, string> campo = retorno.ElementAt(i);
                //insere a coluna no gridView com o apelido definido
                gridView.Columns.Add(campo.Key, campo.Value);
                //verifica qual eh o índice da tabelaSelect correspondente ao campo para ordenar depois
                if (tabela != null)
                {
                    for (int j = 0; j < tabela.Campos.Length; j++)
                    {
                        if (tabela.Campos[j].Equals(campo.Key))
                        {
                            indices[i] = j;
                            break;
                        }
                    }
                }
            }

            if (tabela == null)
            {
                string[] linha = new string[retorno.Count];
                gridView.Rows.Add(linha);
            }
            else
                foreach (string[] registro in tabela.Registros)
                {
                    string[] linha = new string[registro.Length];
                    for (int i = 0; i < indices.Length; i++)
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

        private void btnLimpa_Click(object sender, EventArgs e)
        {
            txtComando.Clear();
            txtComando.Refresh();
            txtComando.Focus();
        }

        private void btn_CarregaArquivo_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName))
                {
                    executa(sr.ReadToEnd());
                    sr.Close();
                }
            }
        }
    }
}

﻿namespace BancoDeDadosPOD
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtComando = new System.Windows.Forms.TextBox();
            this.btnExecuta = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabResultado = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtMensagens = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnLimpa = new System.Windows.Forms.Button();
            this.btn_CarregaArquivo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtQtdRegistros = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabResultado.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtComando
            // 
            this.txtComando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComando.Location = new System.Drawing.Point(13, 13);
            this.txtComando.Multiline = true;
            this.txtComando.Name = "txtComando";
            this.txtComando.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtComando.Size = new System.Drawing.Size(675, 164);
            this.txtComando.TabIndex = 0;
            // 
            // btnExecuta
            // 
            this.btnExecuta.Location = new System.Drawing.Point(12, 183);
            this.btnExecuta.Name = "btnExecuta";
            this.btnExecuta.Size = new System.Drawing.Size(75, 23);
            this.btnExecuta.TabIndex = 1;
            this.btnExecuta.Text = "Executa";
            this.btnExecuta.UseVisualStyleBackColor = true;
            this.btnExecuta.Click += new System.EventHandler(this.btnExecuta_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(658, 191);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // tabResultado
            // 
            this.tabResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabResultado.Controls.Add(this.tabPage1);
            this.tabResultado.Controls.Add(this.tabPage2);
            this.tabResultado.Location = new System.Drawing.Point(13, 213);
            this.tabResultado.Name = "tabResultado";
            this.tabResultado.SelectedIndex = 0;
            this.tabResultado.Size = new System.Drawing.Size(675, 229);
            this.tabResultado.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtMensagens);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(667, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mensagens";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtMensagens
            // 
            this.txtMensagens.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMensagens.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtMensagens.Location = new System.Drawing.Point(4, 4);
            this.txtMensagens.Multiline = true;
            this.txtMensagens.Name = "txtMensagens";
            this.txtMensagens.ReadOnly = true;
            this.txtMensagens.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMensagens.Size = new System.Drawing.Size(660, 196);
            this.txtMensagens.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(667, 203);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Resultado";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnLimpa
            // 
            this.btnLimpa.Location = new System.Drawing.Point(94, 184);
            this.btnLimpa.Name = "btnLimpa";
            this.btnLimpa.Size = new System.Drawing.Size(75, 23);
            this.btnLimpa.TabIndex = 4;
            this.btnLimpa.Text = "Limpa";
            this.btnLimpa.UseVisualStyleBackColor = true;
            this.btnLimpa.Click += new System.EventHandler(this.btnLimpa_Click);
            // 
            // btn_CarregaArquivo
            // 
            this.btn_CarregaArquivo.Location = new System.Drawing.Point(588, 184);
            this.btn_CarregaArquivo.Name = "btn_CarregaArquivo";
            this.btn_CarregaArquivo.Size = new System.Drawing.Size(99, 23);
            this.btn_CarregaArquivo.TabIndex = 5;
            this.btn_CarregaArquivo.Text = "Carrega Arquivo";
            this.btn_CarregaArquivo.UseVisualStyleBackColor = true;
            this.btn_CarregaArquivo.Click += new System.EventHandler(this.btn_CarregaArquivo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 190);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Quantidade Máxima de Registros:";
            // 
            // txtQtdRegistros
            // 
            this.txtQtdRegistros.Location = new System.Drawing.Point(482, 187);
            this.txtQtdRegistros.Mask = "999999";
            this.txtQtdRegistros.Name = "txtQtdRegistros";
            this.txtQtdRegistros.Size = new System.Drawing.Size(100, 20);
            this.txtQtdRegistros.TabIndex = 8;
            this.txtQtdRegistros.Text = "40000";
            this.txtQtdRegistros.TextChanged += new System.EventHandler(this.txtQtdRegistros_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 454);
            this.Controls.Add(this.txtQtdRegistros);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_CarregaArquivo);
            this.Controls.Add(this.btnLimpa);
            this.Controls.Add(this.tabResultado);
            this.Controls.Add(this.btnExecuta);
            this.Controls.Add(this.txtComando);
            this.Name = "Form1";
            this.Text = "BDPod";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabResultado.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComando;
        private System.Windows.Forms.Button btnExecuta;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabControl tabResultado;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtMensagens;
        private System.Windows.Forms.Button btnLimpa;
        private System.Windows.Forms.Button btn_CarregaArquivo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtQtdRegistros;
    }
}


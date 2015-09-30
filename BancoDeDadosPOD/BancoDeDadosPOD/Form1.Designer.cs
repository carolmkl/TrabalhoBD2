namespace BancoDeDadosPOD
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtComando
            // 
            this.txtComando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComando.Location = new System.Drawing.Point(13, 13);
            this.txtComando.Multiline = true;
            this.txtComando.Name = "txtComando";
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
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 212);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(675, 230);
            this.dataGridView1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 454);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnExecuta);
            this.Controls.Add(this.txtComando);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComando;
        private System.Windows.Forms.Button btnExecuta;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}


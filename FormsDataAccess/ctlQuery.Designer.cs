namespace FormsDataAccess
{
    partial class ctlQuery
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtConnect = new System.Windows.Forms.TextBox();
            this.dgvResults = new MDDWinForms.DataGridViewRowUpdate();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.btnExecute);
            this.scMain.Panel1.Controls.Add(this.txtQuery);
            this.scMain.Panel1.Controls.Add(this.btnConnect);
            this.scMain.Panel1.Controls.Add(this.txtConnect);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.dgvResults);
            this.scMain.Size = new System.Drawing.Size(894, 458);
            this.scMain.SplitterDistance = 194;
            this.scMain.SplitterWidth = 3;
            this.scMain.TabIndex = 0;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(216, 1);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(50, 23);
            this.btnExecute.TabIndex = 4;
            this.btnExecute.Text = "Run";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtQuery
            // 
            this.txtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuery.Location = new System.Drawing.Point(0, 23);
            this.txtQuery.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(893, 171);
            this.txtQuery.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(148, 1);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(64, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtConnect
            // 
            this.txtConnect.Location = new System.Drawing.Point(0, 2);
            this.txtConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtConnect.Name = "txtConnect";
            this.txtConnect.Size = new System.Drawing.Size(145, 20);
            this.txtConnect.TabIndex = 0;
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.FilterTextBox = null;
            this.dgvResults.GetContextMenu = null;
            this.dgvResults.Location = new System.Drawing.Point(0, 0);
            this.dgvResults.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 62;
            this.dgvResults.RowTemplate.Height = 28;
            this.dgvResults.Size = new System.Drawing.Size(894, 261);
            this.dgvResults.TabIndex = 0;
            this.dgvResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvResults_DataBindingComplete);
            this.dgvResults.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvResults_DataError);
            // 
            // ctlQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scMain);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ctlQuery";
            this.Size = new System.Drawing.Size(894, 458);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel1.PerformLayout();
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TextBox txtConnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtQuery;
        private MDDWinForms.DataGridViewRowUpdate dgvResults;
    }
}

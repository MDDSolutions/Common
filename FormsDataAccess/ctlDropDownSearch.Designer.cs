namespace FormsDataAccess
{
    partial class ctlDropDownSearch
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
            this.components = new System.ComponentModel.Container();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lbxListItems = new System.Windows.Forms.ListBox();
            this.bsListItems = new System.Windows.Forms.BindingSource(this.components);
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsListItems)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControls.Controls.Add(this.label1);
            this.pnlControls.Controls.Add(this.btnClear);
            this.pnlControls.Controls.Add(this.txtSearch);
            this.pnlControls.Controls.Add(this.lbxListItems);
            this.pnlControls.Location = new System.Drawing.Point(0, 21);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(312, 230);
            this.pnlControls.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Search:";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(286, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(26, 20);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "x";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(44, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(242, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lbxListItems
            // 
            this.lbxListItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxListItems.DataSource = this.bsListItems;
            this.lbxListItems.FormattingEnabled = true;
            this.lbxListItems.Location = new System.Drawing.Point(0, 20);
            this.lbxListItems.Name = "lbxListItems";
            this.lbxListItems.Size = new System.Drawing.Size(312, 212);
            this.lbxListItems.TabIndex = 0;
            this.lbxListItems.SelectedIndexChanged += new System.EventHandler(this.lbxAccounts_SelectedIndexChanged);
            // 
            // bsListItems
            // 
            this.bsListItems.CurrentItemChanged += new System.EventHandler(this.bsListItems_CurrentItemChanged);
            // 
            // ctlDropDownSearch
            // 
            this.AnchorSize = new System.Drawing.Size(312, 21);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControls);
            this.Name = "ctlDropDownSearch";
            this.Size = new System.Drawing.Size(312, 251);
            this.Dropping += new System.EventHandler(this.ctlDropDownSearch_Dropping);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsListItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListBox lbxListItems;
        private System.Windows.Forms.BindingSource bsListItems;
    }
}

namespace FormsDataAccess
{
    partial class frmEditor
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.bsMain = new System.Windows.Forms.BindingSource(this.components);
            this.chkCloseOnSave = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(293, 341);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(212, 341);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 27;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkCloseOnSave
            // 
            this.chkCloseOnSave.AutoSize = true;
            this.chkCloseOnSave.Checked = true;
            this.chkCloseOnSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCloseOnSave.Location = new System.Drawing.Point(212, 318);
            this.chkCloseOnSave.Name = "chkCloseOnSave";
            this.chkCloseOnSave.Size = new System.Drawing.Size(95, 17);
            this.chkCloseOnSave.TabIndex = 29;
            this.chkCloseOnSave.Text = "Close on Save";
            this.chkCloseOnSave.UseVisualStyleBackColor = true;
            // 
            // frmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 704);
            this.Controls.Add(this.chkCloseOnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "frmEditor";
            this.Text = "frmEditor";
            this.Load += new System.EventHandler(this.frmEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.BindingSource bsMain;
        protected System.Windows.Forms.CheckBox chkCloseOnSave;
    }
}
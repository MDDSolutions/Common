namespace MDDWinForms
{
    partial class PictureBoxWithTextBox
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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                Reset();
                base.Dispose(disposing);
            }
            catch { }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pbx = new System.Windows.Forms.PictureBox();
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.txt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).BeginInit();
            this.cms.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbx
            // 
            this.pbx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbx.ContextMenuStrip = this.cms;
            this.pbx.Location = new System.Drawing.Point(0, 0);
            this.pbx.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbx.Name = "pbx";
            this.pbx.Size = new System.Drawing.Size(309, 422);
            this.pbx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbx.TabIndex = 0;
            this.pbx.TabStop = false;
            this.pbx.Click += new System.EventHandler(this.pbx_Click);
            // 
            // cms
            // 
            this.cms.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmCopyImage,
            this.tsmSelect});
            this.cms.Name = "cms";
            this.cms.Size = new System.Drawing.Size(182, 68);
            this.cms.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // tsmCopyImage
            // 
            this.tsmCopyImage.Name = "tsmCopyImage";
            this.tsmCopyImage.Size = new System.Drawing.Size(181, 32);
            this.tsmCopyImage.Text = "Copy Image";
            this.tsmCopyImage.Click += new System.EventHandler(this.CopyImage);
            // 
            // tsmSelect
            // 
            this.tsmSelect.Name = "tsmSelect";
            this.tsmSelect.Size = new System.Drawing.Size(181, 32);
            this.tsmSelect.Text = "Select";
            this.tsmSelect.Click += new System.EventHandler(this.tsmSelect_Click);
            // 
            // txt
            // 
            this.txt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt.Location = new System.Drawing.Point(0, 422);
            this.txt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(307, 26);
            this.txt.TabIndex = 1;
            // 
            // PictureBoxWithTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt);
            this.Controls.Add(this.pbx);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PictureBoxWithTextBox";
            this.Size = new System.Drawing.Size(309, 452);
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).EndInit();
            this.cms.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pbx;
        public System.Windows.Forms.TextBox txt;
        private System.Windows.Forms.ContextMenuStrip cms;
        private System.Windows.Forms.ToolStripMenuItem tsmCopyImage;
        private System.Windows.Forms.ToolStripMenuItem tsmSelect;
    }
}

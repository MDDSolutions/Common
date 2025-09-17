namespace MDDWinForms
{
    partial class frmImageEdit
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
            this.pbx = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnGetFromClipboard = new System.Windows.Forms.Button();
            this.txtIncrement = new System.Windows.Forms.TextBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnCrop = new System.Windows.Forms.Button();
            this.chkCrop = new System.Windows.Forms.CheckBox();
            this.tmrMain = new System.Windows.Forms.Timer(this.components);
            this.Label1 = new System.Windows.Forms.Label();
            this.txtDPIFactor = new System.Windows.Forms.TextBox();
            this.txtXOffset = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).BeginInit();
            this.SuspendLayout();
            // 
            // pbx
            // 
            this.pbx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbx.Location = new System.Drawing.Point(0, 0);
            this.pbx.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbx.Name = "pbx";
            this.pbx.Size = new System.Drawing.Size(1377, 842);
            this.pbx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbx.TabIndex = 0;
            this.pbx.TabStop = false;
            this.pbx.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseDown);
            this.pbx.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseMove);
            this.pbx.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseUp);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(1246, 852);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(1125, 852);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnGetFromClipboard
            // 
            this.btnGetFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetFromClipboard.Location = new System.Drawing.Point(18, 852);
            this.btnGetFromClipboard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnGetFromClipboard.Name = "btnGetFromClipboard";
            this.btnGetFromClipboard.Size = new System.Drawing.Size(159, 35);
            this.btnGetFromClipboard.TabIndex = 3;
            this.btnGetFromClipboard.Text = "Get From Clipboard";
            this.btnGetFromClipboard.UseVisualStyleBackColor = true;
            this.btnGetFromClipboard.Click += new System.EventHandler(this.btnGetFromClipboard_Click);
            // 
            // txtIncrement
            // 
            this.txtIncrement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtIncrement.Location = new System.Drawing.Point(356, 857);
            this.txtIncrement.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtIncrement.Name = "txtIncrement";
            this.txtIncrement.Size = new System.Drawing.Size(36, 26);
            this.txtIncrement.TabIndex = 4;
            this.txtIncrement.Text = "5";
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLeft.Location = new System.Drawing.Point(402, 852);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(78, 35);
            this.btnLeft.TabIndex = 6;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRight.Location = new System.Drawing.Point(489, 852);
            this.btnRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(78, 35);
            this.btnRight.TabIndex = 7;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnCrop
            // 
            this.btnCrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrop.Location = new System.Drawing.Point(1004, 852);
            this.btnCrop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(112, 35);
            this.btnCrop.TabIndex = 8;
            this.btnCrop.Text = "Crop";
            this.btnCrop.UseVisualStyleBackColor = true;
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // chkCrop
            // 
            this.chkCrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCrop.AutoSize = true;
            this.chkCrop.Location = new System.Drawing.Point(186, 862);
            this.chkCrop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkCrop.Name = "chkCrop";
            this.chkCrop.Size = new System.Drawing.Size(158, 24);
            this.chkCrop.TabIndex = 9;
            this.chkCrop.Text = "Crop - Increment:";
            this.chkCrop.UseVisualStyleBackColor = true;
            this.chkCrop.CheckedChanged += new System.EventHandler(this.chkCrop_CheckedChanged);
            // 
            // tmrMain
            // 
            this.tmrMain.Enabled = true;
            this.tmrMain.Interval = 250;
            this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(616, 862);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(90, 20);
            this.Label1.TabIndex = 10;
            this.Label1.Text = "DPI Factor:";
            // 
            // txtDPIFactor
            // 
            this.txtDPIFactor.Location = new System.Drawing.Point(712, 859);
            this.txtDPIFactor.Name = "txtDPIFactor";
            this.txtDPIFactor.Size = new System.Drawing.Size(69, 26);
            this.txtDPIFactor.TabIndex = 11;
            this.txtDPIFactor.Text = "0";
            this.txtDPIFactor.TextChanged += new System.EventHandler(this.txtDPIFactor_TextChanged);
            // 
            // txtXOffset
            // 
            this.txtXOffset.Location = new System.Drawing.Point(893, 859);
            this.txtXOffset.Name = "txtXOffset";
            this.txtXOffset.Size = new System.Drawing.Size(69, 26);
            this.txtXOffset.TabIndex = 13;
            this.txtXOffset.Text = "0";
            this.txtXOffset.TextChanged += new System.EventHandler(this.txtXOffset_TextChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(797, 862);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 20);
            this.Label2.TabIndex = 12;
            this.Label2.Text = "X Offset:";
            // 
            // frmImageEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1377, 906);
            this.Controls.Add(this.txtXOffset);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.txtDPIFactor);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.chkCrop);
            this.Controls.Add(this.btnCrop);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.txtIncrement);
            this.Controls.Add(this.btnGetFromClipboard);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbx);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmImageEdit";
            this.Text = "Image Edit";
            this.Load += new System.EventHandler(this.frmImageEdit_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmImageEdit_MouseUp);
            this.Move += new System.EventHandler(this.frmImageEdit_Move);
            this.Resize += new System.EventHandler(this.frmImageEdit_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbx;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnGetFromClipboard;
        private System.Windows.Forms.TextBox txtIncrement;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnCrop;
        private System.Windows.Forms.CheckBox chkCrop;
        private System.Windows.Forms.Timer tmrMain;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox txtDPIFactor;
        private System.Windows.Forms.TextBox txtXOffset;
        private System.Windows.Forms.Label Label2;
    }
}
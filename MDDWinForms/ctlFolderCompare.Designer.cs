namespace MDDWinForms
{
    partial class ctlFolderCompare
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoad1 = new System.Windows.Forms.Button();
            this.btnLoad2 = new System.Windows.Forms.Button();
            this.txtStatus1 = new System.Windows.Forms.TextBox();
            this.txtStatus2 = new System.Windows.Forms.TextBox();
            this.cbxFolder1 = new System.Windows.Forms.ComboBox();
            this.cmsCombobox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxFolder2 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.gbDisplay = new System.Windows.Forms.GroupBox();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbFolder2 = new System.Windows.Forms.RadioButton();
            this.rbFolder1 = new System.Windows.Forms.RadioButton();
            this.rbMatching = new System.Windows.Forms.RadioButton();
            this.dgvResult = new MDDWinForms.DataGridViewRowUpdate();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HashesMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsResult = new System.Windows.Forms.BindingSource(this.components);
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.lbxTasks = new System.Windows.Forms.ListBox();
            this.bsTasks = new System.Windows.Forms.BindingSource(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.tmrMain = new System.Windows.Forms.Timer(this.components);
            this.txtQItems = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmsCombobox.SuspendLayout();
            this.gbDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Folder 1:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Folder 2:";
            // 
            // btnLoad1
            // 
            this.btnLoad1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad1.Location = new System.Drawing.Point(1623, -1);
            this.btnLoad1.Name = "btnLoad1";
            this.btnLoad1.Size = new System.Drawing.Size(75, 32);
            this.btnLoad1.TabIndex = 4;
            this.btnLoad1.Text = "Load";
            this.btnLoad1.UseVisualStyleBackColor = true;
            this.btnLoad1.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnLoad2
            // 
            this.btnLoad2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad2.Location = new System.Drawing.Point(1623, 31);
            this.btnLoad2.Name = "btnLoad2";
            this.btnLoad2.Size = new System.Drawing.Size(75, 32);
            this.btnLoad2.TabIndex = 5;
            this.btnLoad2.Text = "Load";
            this.btnLoad2.UseVisualStyleBackColor = true;
            this.btnLoad2.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtStatus1
            // 
            this.txtStatus1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus1.Location = new System.Drawing.Point(1704, 3);
            this.txtStatus1.Name = "txtStatus1";
            this.txtStatus1.Size = new System.Drawing.Size(131, 26);
            this.txtStatus1.TabIndex = 6;
            // 
            // txtStatus2
            // 
            this.txtStatus2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus2.Location = new System.Drawing.Point(1704, 35);
            this.txtStatus2.Name = "txtStatus2";
            this.txtStatus2.Size = new System.Drawing.Size(131, 26);
            this.txtStatus2.TabIndex = 7;
            // 
            // cbxFolder1
            // 
            this.cbxFolder1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxFolder1.ContextMenuStrip = this.cmsCombobox;
            this.cbxFolder1.FormattingEnabled = true;
            this.cbxFolder1.Location = new System.Drawing.Point(80, 3);
            this.cbxFolder1.Name = "cbxFolder1";
            this.cbxFolder1.Size = new System.Drawing.Size(1537, 28);
            this.cbxFolder1.TabIndex = 11;
            this.cbxFolder1.Validating += new System.ComponentModel.CancelEventHandler(this.cbxFolder_Validating);
            // 
            // cmsCombobox
            // 
            this.cmsCombobox.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsCombobox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.cmsCombobox.Name = "cmsCombobox";
            this.cmsCombobox.Size = new System.Drawing.Size(149, 36);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(148, 32);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // cbxFolder2
            // 
            this.cbxFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxFolder2.ContextMenuStrip = this.cmsCombobox;
            this.cbxFolder2.FormattingEnabled = true;
            this.cbxFolder2.Location = new System.Drawing.Point(80, 35);
            this.cbxFolder2.Name = "cbxFolder2";
            this.cbxFolder2.Size = new System.Drawing.Size(1537, 28);
            this.cbxFolder2.TabIndex = 12;
            this.cbxFolder2.Validating += new System.ComponentModel.CancelEventHandler(this.cbxFolder_Validating);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(897, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gbDisplay
            // 
            this.gbDisplay.Controls.Add(this.rbAll);
            this.gbDisplay.Controls.Add(this.rbFolder2);
            this.gbDisplay.Controls.Add(this.rbFolder1);
            this.gbDisplay.Controls.Add(this.rbMatching);
            this.gbDisplay.Location = new System.Drawing.Point(7, 69);
            this.gbDisplay.Name = "gbDisplay";
            this.gbDisplay.Size = new System.Drawing.Size(374, 65);
            this.gbDisplay.TabIndex = 16;
            this.gbDisplay.TabStop = false;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(6, 25);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(51, 24);
            this.rbAll.TabIndex = 3;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbFolder2
            // 
            this.rbFolder2.AutoSize = true;
            this.rbFolder2.Location = new System.Drawing.Point(267, 25);
            this.rbFolder2.Name = "rbFolder2";
            this.rbFolder2.Size = new System.Drawing.Size(92, 24);
            this.rbFolder2.TabIndex = 2;
            this.rbFolder2.TabStop = true;
            this.rbFolder2.Text = "Folder 2";
            this.rbFolder2.UseVisualStyleBackColor = true;
            this.rbFolder2.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbFolder1
            // 
            this.rbFolder1.AutoSize = true;
            this.rbFolder1.Location = new System.Drawing.Point(169, 25);
            this.rbFolder1.Name = "rbFolder1";
            this.rbFolder1.Size = new System.Drawing.Size(92, 24);
            this.rbFolder1.TabIndex = 1;
            this.rbFolder1.TabStop = true;
            this.rbFolder1.Text = "Folder 1";
            this.rbFolder1.UseVisualStyleBackColor = true;
            this.rbFolder1.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbMatching
            // 
            this.rbMatching.AutoSize = true;
            this.rbMatching.Location = new System.Drawing.Point(64, 25);
            this.rbMatching.Name = "rbMatching";
            this.rbMatching.Size = new System.Drawing.Size(99, 24);
            this.rbMatching.TabIndex = 0;
            this.rbMatching.TabStop = true;
            this.rbMatching.Text = "Matching";
            this.rbMatching.UseVisualStyleBackColor = true;
            this.rbMatching.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // dgvResult
            // 
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.AutoGenerateColumns = false;
            this.dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.HashesMatch});
            this.dgvResult.DataSource = this.bsResult;
            this.dgvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult.FilterTextBox = null;
            this.dgvResult.GetContextMenu = null;
            this.dgvResult.Location = new System.Drawing.Point(0, 0);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.ReadOnly = true;
            this.dgvResult.RowHeadersWidth = 40;
            this.dgvResult.RowTemplate.Height = 28;
            this.dgvResult.Size = new System.Drawing.Size(1108, 1260);
            this.dgvResult.TabIndex = 10;
            this.dgvResult.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dgvResult_CellContextMenuStripNeeded);
            this.dgvResult.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvResult_CellFormatting);
            this.dgvResult.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvResult_ColumnHeaderMouseClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 87;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Length1";
            this.dataGridViewTextBoxColumn4.HeaderText = "Length1";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 104;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Length2";
            this.dataGridViewTextBoxColumn5.HeaderText = "Length2";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 104;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "LastWrite1";
            this.dataGridViewTextBoxColumn6.HeaderText = "LastWrite1";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 122;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "LastWrite2";
            this.dataGridViewTextBoxColumn7.HeaderText = "LastWrite2";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 122;
            // 
            // HashesMatch
            // 
            this.HashesMatch.DataPropertyName = "HashesMatch";
            this.HashesMatch.HeaderText = "HashesMatch";
            this.HashesMatch.MinimumWidth = 8;
            this.HashesMatch.Name = "HashesMatch";
            this.HashesMatch.ReadOnly = true;
            this.HashesMatch.Width = 144;
            // 
            // bsResult
            // 
            this.bsResult.DataSource = typeof(MDDWinForms.ComparisonResult);
            // 
            // scMain
            // 
            this.scMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMain.Location = new System.Drawing.Point(0, 140);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.dgvResult);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.txtOutput);
            this.scMain.Panel2.Controls.Add(this.lbxTasks);
            this.scMain.Size = new System.Drawing.Size(1835, 1260);
            this.scMain.SplitterDistance = 1108;
            this.scMain.TabIndex = 17;
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(2, 230);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(718, 1030);
            this.txtOutput.TabIndex = 1;
            // 
            // lbxTasks
            // 
            this.lbxTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxTasks.DataSource = this.bsTasks;
            this.lbxTasks.FormattingEnabled = true;
            this.lbxTasks.ItemHeight = 20;
            this.lbxTasks.Location = new System.Drawing.Point(2, 0);
            this.lbxTasks.Name = "lbxTasks";
            this.lbxTasks.Size = new System.Drawing.Size(718, 224);
            this.lbxTasks.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(1757, 96);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 38);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tmrMain
            // 
            this.tmrMain.Enabled = true;
            this.tmrMain.Interval = 500;
            this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
            // 
            // txtQItems
            // 
            this.txtQItems.Location = new System.Drawing.Point(1651, 101);
            this.txtQItems.Name = "txtQItems";
            this.txtQItems.Size = new System.Drawing.Size(100, 26);
            this.txtQItems.TabIndex = 19;
            this.txtQItems.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1524, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 20);
            this.label3.TabIndex = 20;
            this.label3.Text = "Items in Queue:";
            // 
            // ctlFolderCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtQItems);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.gbDisplay);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbxFolder2);
            this.Controls.Add(this.cbxFolder1);
            this.Controls.Add(this.txtStatus2);
            this.Controls.Add(this.txtStatus1);
            this.Controls.Add(this.btnLoad2);
            this.Controls.Add(this.btnLoad1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ctlFolderCompare";
            this.Size = new System.Drawing.Size(1840, 1400);
            this.cmsCombobox.ResumeLayout(false);
            this.gbDisplay.ResumeLayout(false);
            this.gbDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsResult)).EndInit();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            this.scMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsTasks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad1;
        private System.Windows.Forms.Button btnLoad2;
        private System.Windows.Forms.TextBox txtStatus1;
        private System.Windows.Forms.TextBox txtStatus2;
        private System.Windows.Forms.BindingSource bsResult;
        private DataGridViewRowUpdate dgvResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.ComboBox cbxFolder1;
        private System.Windows.Forms.ComboBox cbxFolder2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn HashesMatch;
        private System.Windows.Forms.GroupBox gbDisplay;
        private System.Windows.Forms.RadioButton rbFolder1;
        private System.Windows.Forms.RadioButton rbMatching;
        private System.Windows.Forms.RadioButton rbFolder2;
        private System.Windows.Forms.ContextMenuStrip cmsCombobox;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ListBox lbxTasks;
        private System.Windows.Forms.BindingSource bsTasks;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer tmrMain;
        private System.Windows.Forms.TextBox txtQItems;
        private System.Windows.Forms.Label label3;
    }
}

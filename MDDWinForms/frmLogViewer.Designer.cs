namespace MDDWinForms
{
    partial class frmLogViewer
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ComboBox cboLogs;
        private TextBoxPlaceHolder txtSource;
        private TextBoxPlaceHolder txtAssembly;
        private TextBoxPlaceHolder txtClass;
        private TextBoxPlaceHolder txtMethod;
        private TextBoxPlaceHolder txtKeyword;
        private System.Windows.Forms.Button btnApplyFilter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cboLogs = new System.Windows.Forms.ComboBox();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtSeverity = new MDDWinForms.TextBoxPlaceHolder();
            this.txtSince = new MDDWinForms.TextBoxPlaceHolder();
            this.txtKeyword = new MDDWinForms.TextBoxPlaceHolder();
            this.txtMethod = new MDDWinForms.TextBoxPlaceHolder();
            this.txtClass = new MDDWinForms.TextBoxPlaceHolder();
            this.txtAssembly = new MDDWinForms.TextBoxPlaceHolder();
            this.txtSource = new MDDWinForms.TextBoxPlaceHolder();
            this.bsEntries = new System.Windows.Forms.BindingSource(this.components);
            this.dgvEntries = new MDDWinForms.DataGridViewRowUpdate();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bsEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).BeginInit();
            this.SuspendLayout();
            // 
            // cboLogs
            // 
            this.cboLogs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogs.FormattingEnabled = true;
            this.cboLogs.Location = new System.Drawing.Point(12, 12);
            this.cboLogs.Name = "cboLogs";
            this.cboLogs.Size = new System.Drawing.Size(200, 28);
            this.cboLogs.TabIndex = 0;
            this.cboLogs.SelectedIndexChanged += new System.EventHandler(this.cboLogs_SelectedIndexChanged);
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Location = new System.Drawing.Point(983, 8);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(84, 30);
            this.btnApplyFilter.TabIndex = 8;
            this.btnApplyFilter.Text = "Apply";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Location = new System.Drawing.Point(1083, 10);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(240, 26);
            this.txtStatus.TabIndex = 9;
            this.txtStatus.TabStop = false;
            this.txtStatus.Text = "Not Active";
            // 
            // txtSeverity
            // 
            this.txtSeverity.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtSeverity.Location = new System.Drawing.Point(239, 10);
            this.txtSeverity.Name = "txtSeverity";
            this.txtSeverity.PlaceHolderText = "Severity";
            this.txtSeverity.Size = new System.Drawing.Size(100, 26);
            this.txtSeverity.TabIndex = 1;
            this.txtSeverity.Text = null;
            this.txtSeverity.Validating += new System.ComponentModel.CancelEventHandler(this.txtSeverity_Validating);
            this.txtSeverity.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtSince
            // 
            this.txtSince.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtSince.Location = new System.Drawing.Point(345, 10);
            this.txtSince.Name = "txtSince";
            this.txtSince.PlaceHolderText = "Since";
            this.txtSince.Size = new System.Drawing.Size(100, 26);
            this.txtSince.TabIndex = 2;
            this.txtSince.Text = null;
            this.txtSince.Validating += new System.ComponentModel.CancelEventHandler(this.txtSince_Validating);
            this.txtSince.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtKeyword
            // 
            this.txtKeyword.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtKeyword.Location = new System.Drawing.Point(877, 10);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.PlaceHolderText = "Keyword";
            this.txtKeyword.Size = new System.Drawing.Size(100, 26);
            this.txtKeyword.TabIndex = 7;
            this.txtKeyword.Text = null;
            this.txtKeyword.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtMethod
            // 
            this.txtMethod.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtMethod.Location = new System.Drawing.Point(771, 10);
            this.txtMethod.Name = "txtMethod";
            this.txtMethod.PlaceHolderText = "Method";
            this.txtMethod.Size = new System.Drawing.Size(100, 26);
            this.txtMethod.TabIndex = 6;
            this.txtMethod.Text = null;
            this.txtMethod.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtClass
            // 
            this.txtClass.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtClass.Location = new System.Drawing.Point(665, 10);
            this.txtClass.Name = "txtClass";
            this.txtClass.PlaceHolderText = "Class";
            this.txtClass.Size = new System.Drawing.Size(100, 26);
            this.txtClass.TabIndex = 5;
            this.txtClass.Text = null;
            this.txtClass.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtAssembly
            // 
            this.txtAssembly.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtAssembly.Location = new System.Drawing.Point(559, 10);
            this.txtAssembly.Name = "txtAssembly";
            this.txtAssembly.PlaceHolderText = "Assembly";
            this.txtAssembly.Size = new System.Drawing.Size(100, 26);
            this.txtAssembly.TabIndex = 4;
            this.txtAssembly.Text = null;
            this.txtAssembly.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtSource
            // 
            this.txtSource.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtSource.Location = new System.Drawing.Point(453, 10);
            this.txtSource.Name = "txtSource";
            this.txtSource.PlaceHolderText = "Source";
            this.txtSource.Size = new System.Drawing.Size(100, 26);
            this.txtSource.TabIndex = 3;
            this.txtSource.Text = null;
            this.txtSource.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // bsEntries
            // 
            this.bsEntries.DataSource = typeof(MDDFoundation.RichLogEntry);
            this.bsEntries.CurrentChanged += new System.EventHandler(this.bsEntries_CurrentChanged);
            // 
            // dgvEntries
            // 
            this.dgvEntries.AllowUserToAddRows = false;
            this.dgvEntries.AllowUserToDeleteRows = false;
            this.dgvEntries.AutoGenerateColumns = false;
            this.dgvEntries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7});
            this.dgvEntries.DataSource = this.bsEntries;
            this.dgvEntries.FilterTextBox = null;
            this.dgvEntries.GetContextMenu = null;
            this.dgvEntries.Location = new System.Drawing.Point(12, 46);
            this.dgvEntries.Name = "dgvEntries";
            this.dgvEntries.ReadOnly = true;
            this.dgvEntries.RowHeadersWidth = 62;
            this.dgvEntries.RowTemplate.Height = 28;
            this.dgvEntries.Size = new System.Drawing.Size(1798, 835);
            this.dgvEntries.TabIndex = 11;
            this.dgvEntries.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEntries_CellDoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Timestamp";
            this.dataGridViewTextBoxColumn1.HeaderText = "Timestamp";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 123;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Source";
            this.dataGridViewTextBoxColumn2.HeaderText = "Source";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 96;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Severity";
            this.dataGridViewTextBoxColumn3.HeaderText = "Severity";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 101;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Message";
            this.dataGridViewTextBoxColumn4.HeaderText = "Message";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 110;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "AssemblyName";
            this.dataGridViewTextBoxColumn5.HeaderText = "AssemblyName";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 155;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ClassName";
            this.dataGridViewTextBoxColumn6.HeaderText = "ClassName";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 126;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "MethodName";
            this.dataGridViewTextBoxColumn7.HeaderText = "MethodName";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 141;
            // 
            // frmLogViewer
            // 
            this.ClientSize = new System.Drawing.Size(1822, 893);
            this.Controls.Add(this.dgvEntries);
            this.Controls.Add(this.txtSeverity);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtSince);
            this.Controls.Add(this.btnApplyFilter);
            this.Controls.Add(this.txtKeyword);
            this.Controls.Add(this.txtMethod);
            this.Controls.Add(this.txtClass);
            this.Controls.Add(this.txtAssembly);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.cboLogs);
            this.Name = "frmLogViewer";
            this.Text = "RichLog Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.bsEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private TextBoxPlaceHolder txtSince;
        private System.Windows.Forms.TextBox txtStatus;
        private TextBoxPlaceHolder txtSeverity;
        private System.Windows.Forms.BindingSource bsEntries;
        private DataGridViewRowUpdate dgvEntries;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    }
}

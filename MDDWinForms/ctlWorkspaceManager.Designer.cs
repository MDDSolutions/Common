namespace MDDWinForms
{
    partial class ctlWorkspaceManager
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
            this.lbxWorkspaces = new System.Windows.Forms.ListBox();
            this.btnGetCurrentState = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.windowStateDataGridViewRowUpdate = new MDDWinForms.DataGridViewRowUpdate();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewCheckBoxColumn2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsWindowState = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.windowStateDataGridViewRowUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsWindowState)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxWorkspaces
            // 
            this.lbxWorkspaces.FormattingEnabled = true;
            this.lbxWorkspaces.ItemHeight = 20;
            this.lbxWorkspaces.Location = new System.Drawing.Point(13, 14);
            this.lbxWorkspaces.Name = "lbxWorkspaces";
            this.lbxWorkspaces.Size = new System.Drawing.Size(385, 204);
            this.lbxWorkspaces.TabIndex = 0;
            this.lbxWorkspaces.SelectedIndexChanged += new System.EventHandler(this.lbxWorkspaces_SelectedIndexChanged);
            this.lbxWorkspaces.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbxWorkspaces_KeyDown);
            // 
            // btnGetCurrentState
            // 
            this.btnGetCurrentState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetCurrentState.Location = new System.Drawing.Point(659, 14);
            this.btnGetCurrentState.Name = "btnGetCurrentState";
            this.btnGetCurrentState.Size = new System.Drawing.Size(267, 34);
            this.btnGetCurrentState.TabIndex = 3;
            this.btnGetCurrentState.Text = "Get Current State";
            this.btnGetCurrentState.UseVisualStyleBackColor = true;
            this.btnGetCurrentState.Click += new System.EventHandler(this.btnGetCurrentState_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(414, 198);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(475, 195);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(364, 26);
            this.txtName.TabIndex = 5;
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(845, 195);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(81, 26);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(404, 14);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(228, 34);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // windowStateDataGridViewRowUpdate
            // 
            this.windowStateDataGridViewRowUpdate.AllowUserToAddRows = false;
            this.windowStateDataGridViewRowUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.windowStateDataGridViewRowUpdate.AutoGenerateColumns = false;
            this.windowStateDataGridViewRowUpdate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.windowStateDataGridViewRowUpdate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewCheckBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.windowStateDataGridViewRowUpdate.DataSource = this.bsWindowState;
            this.windowStateDataGridViewRowUpdate.FilterTextBox = null;
            this.windowStateDataGridViewRowUpdate.GetContextMenu = null;
            this.windowStateDataGridViewRowUpdate.Location = new System.Drawing.Point(13, 242);
            this.windowStateDataGridViewRowUpdate.Name = "windowStateDataGridViewRowUpdate";
            this.windowStateDataGridViewRowUpdate.ReadOnly = true;
            this.windowStateDataGridViewRowUpdate.RowHeadersWidth = 62;
            this.windowStateDataGridViewRowUpdate.RowTemplate.Height = 28;
            this.windowStateDataGridViewRowUpdate.Size = new System.Drawing.Size(913, 614);
            this.windowStateDataGridViewRowUpdate.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "WindowName";
            this.dataGridViewTextBoxColumn1.HeaderText = "WindowName";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Bounds";
            this.dataGridViewTextBoxColumn2.HeaderText = "Bounds";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.DataPropertyName = "IsMaximized";
            this.dataGridViewCheckBoxColumn1.HeaderText = "IsMaximized";
            this.dataGridViewCheckBoxColumn1.MinimumWidth = 8;
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewCheckBoxColumn1.Width = 150;
            // 
            // dataGridViewCheckBoxColumn2
            // 
            this.dataGridViewCheckBoxColumn2.DataPropertyName = "IsMinimized";
            this.dataGridViewCheckBoxColumn2.HeaderText = "IsMinimized";
            this.dataGridViewCheckBoxColumn2.MinimumWidth = 8;
            this.dataGridViewCheckBoxColumn2.Name = "dataGridViewCheckBoxColumn2";
            this.dataGridViewCheckBoxColumn2.ReadOnly = true;
            this.dataGridViewCheckBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "WorkspaceState";
            this.dataGridViewTextBoxColumn3.HeaderText = "WorkspaceState";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "FormTypeName";
            this.dataGridViewTextBoxColumn4.HeaderText = "FormTypeName";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "FormAssemblyName";
            this.dataGridViewTextBoxColumn5.HeaderText = "FormAssemblyName";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // bsWindowState
            // 
            this.bsWindowState.DataSource = typeof(MDDWinForms.WindowState);
            // 
            // ctlWorkspaceManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGetCurrentState);
            this.Controls.Add(this.windowStateDataGridViewRowUpdate);
            this.Controls.Add(this.lbxWorkspaces);
            this.Name = "ctlWorkspaceManager";
            this.Size = new System.Drawing.Size(943, 859);
            this.Load += new System.EventHandler(this.ctlWorkspaceManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.windowStateDataGridViewRowUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsWindowState)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxWorkspaces;
        private System.Windows.Forms.BindingSource bsWindowState;
        private DataGridViewRowUpdate windowStateDataGridViewRowUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Button btnGetCurrentState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnApply;
    }
}

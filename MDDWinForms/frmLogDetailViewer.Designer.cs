namespace MDDWinForms
{
    partial class frmLogDetailViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogDetailViewer));
            System.Windows.Forms.Label assemblyNameLabel;
            System.Windows.Forms.Label classNameLabel;
            System.Windows.Forms.Label detailsLabel;
            System.Windows.Forms.Label messageLabel;
            System.Windows.Forms.Label methodNameLabel;
            System.Windows.Forms.Label severityLabel;
            System.Windows.Forms.Label sourceLabel;
            System.Windows.Forms.Label timestampLabel;
            this.richLogEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.richLogEntryBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.richLogEntryBindingNavigatorSaveItem = new System.Windows.Forms.ToolStripButton();
            this.assemblyNameTextBox = new System.Windows.Forms.TextBox();
            this.classNameTextBox = new System.Windows.Forms.TextBox();
            this.detailsTextBox = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.methodNameTextBox = new System.Windows.Forms.TextBox();
            this.severityTextBox = new System.Windows.Forms.TextBox();
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.timestampTextBox = new System.Windows.Forms.TextBox();
            assemblyNameLabel = new System.Windows.Forms.Label();
            classNameLabel = new System.Windows.Forms.Label();
            detailsLabel = new System.Windows.Forms.Label();
            messageLabel = new System.Windows.Forms.Label();
            methodNameLabel = new System.Windows.Forms.Label();
            severityLabel = new System.Windows.Forms.Label();
            sourceLabel = new System.Windows.Forms.Label();
            timestampLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingNavigator)).BeginInit();
            this.richLogEntryBindingNavigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // richLogEntryBindingSource
            // 
            this.richLogEntryBindingSource.DataSource = typeof(MDDFoundation.RichLogEntry);
            // 
            // richLogEntryBindingNavigator
            // 
            this.richLogEntryBindingNavigator.AddNewItem = this.bindingNavigatorAddNewItem;
            this.richLogEntryBindingNavigator.BindingSource = this.richLogEntryBindingSource;
            this.richLogEntryBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.richLogEntryBindingNavigator.DeleteItem = this.bindingNavigatorDeleteItem;
            this.richLogEntryBindingNavigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.richLogEntryBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.richLogEntryBindingNavigatorSaveItem});
            this.richLogEntryBindingNavigator.Location = new System.Drawing.Point(0, 0);
            this.richLogEntryBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.richLogEntryBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.richLogEntryBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.richLogEntryBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.richLogEntryBindingNavigator.Name = "richLogEntryBindingNavigator";
            this.richLogEntryBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.richLogEntryBindingNavigator.Size = new System.Drawing.Size(814, 38);
            this.richLogEntryBindingNavigator.TabIndex = 0;
            this.richLogEntryBindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(34, 30);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(34, 30);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 31);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(54, 30);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(34, 30);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(34, 30);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(34, 30);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // richLogEntryBindingNavigatorSaveItem
            // 
            this.richLogEntryBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.richLogEntryBindingNavigatorSaveItem.Enabled = false;
            this.richLogEntryBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("richLogEntryBindingNavigatorSaveItem.Image")));
            this.richLogEntryBindingNavigatorSaveItem.Name = "richLogEntryBindingNavigatorSaveItem";
            this.richLogEntryBindingNavigatorSaveItem.Size = new System.Drawing.Size(34, 30);
            this.richLogEntryBindingNavigatorSaveItem.Text = "Save Data";
            // 
            // assemblyNameLabel
            // 
            assemblyNameLabel.AutoSize = true;
            assemblyNameLabel.Location = new System.Drawing.Point(451, 51);
            assemblyNameLabel.Name = "assemblyNameLabel";
            assemblyNameLabel.Size = new System.Drawing.Size(127, 20);
            assemblyNameLabel.TabIndex = 1;
            assemblyNameLabel.Text = "Assembly Name:";
            // 
            // assemblyNameTextBox
            // 
            this.assemblyNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "AssemblyName", true));
            this.assemblyNameTextBox.Location = new System.Drawing.Point(584, 48);
            this.assemblyNameTextBox.Name = "assemblyNameTextBox";
            this.assemblyNameTextBox.Size = new System.Drawing.Size(200, 26);
            this.assemblyNameTextBox.TabIndex = 2;
            // 
            // classNameLabel
            // 
            classNameLabel.AutoSize = true;
            classNameLabel.Location = new System.Drawing.Point(451, 83);
            classNameLabel.Name = "classNameLabel";
            classNameLabel.Size = new System.Drawing.Size(98, 20);
            classNameLabel.TabIndex = 3;
            classNameLabel.Text = "Class Name:";
            // 
            // classNameTextBox
            // 
            this.classNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "ClassName", true));
            this.classNameTextBox.Location = new System.Drawing.Point(584, 80);
            this.classNameTextBox.Name = "classNameTextBox";
            this.classNameTextBox.Size = new System.Drawing.Size(200, 26);
            this.classNameTextBox.TabIndex = 4;
            // 
            // detailsLabel
            // 
            detailsLabel.AutoSize = true;
            detailsLabel.Location = new System.Drawing.Point(17, 193);
            detailsLabel.Name = "detailsLabel";
            detailsLabel.Size = new System.Drawing.Size(62, 20);
            detailsLabel.TabIndex = 5;
            detailsLabel.Text = "Details:";
            // 
            // detailsTextBox
            // 
            this.detailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Details", true));
            this.detailsTextBox.Location = new System.Drawing.Point(150, 190);
            this.detailsTextBox.Multiline = true;
            this.detailsTextBox.Name = "detailsTextBox";
            this.detailsTextBox.Size = new System.Drawing.Size(652, 356);
            this.detailsTextBox.TabIndex = 6;
            // 
            // messageLabel
            // 
            messageLabel.AutoSize = true;
            messageLabel.Location = new System.Drawing.Point(17, 144);
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new System.Drawing.Size(78, 20);
            messageLabel.TabIndex = 7;
            messageLabel.Text = "Message:";
            // 
            // messageTextBox
            // 
            this.messageTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Message", true));
            this.messageTextBox.Location = new System.Drawing.Point(150, 141);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(200, 26);
            this.messageTextBox.TabIndex = 8;
            // 
            // methodNameLabel
            // 
            methodNameLabel.AutoSize = true;
            methodNameLabel.Location = new System.Drawing.Point(451, 115);
            methodNameLabel.Name = "methodNameLabel";
            methodNameLabel.Size = new System.Drawing.Size(113, 20);
            methodNameLabel.TabIndex = 9;
            methodNameLabel.Text = "Method Name:";
            // 
            // methodNameTextBox
            // 
            this.methodNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "MethodName", true));
            this.methodNameTextBox.Location = new System.Drawing.Point(584, 112);
            this.methodNameTextBox.Name = "methodNameTextBox";
            this.methodNameTextBox.Size = new System.Drawing.Size(200, 26);
            this.methodNameTextBox.TabIndex = 10;
            // 
            // severityLabel
            // 
            severityLabel.AutoSize = true;
            severityLabel.Location = new System.Drawing.Point(17, 112);
            severityLabel.Name = "severityLabel";
            severityLabel.Size = new System.Drawing.Size(69, 20);
            severityLabel.TabIndex = 11;
            severityLabel.Text = "Severity:";
            // 
            // severityTextBox
            // 
            this.severityTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Severity", true));
            this.severityTextBox.Location = new System.Drawing.Point(150, 109);
            this.severityTextBox.Name = "severityTextBox";
            this.severityTextBox.Size = new System.Drawing.Size(200, 26);
            this.severityTextBox.TabIndex = 12;
            // 
            // sourceLabel
            // 
            sourceLabel.AutoSize = true;
            sourceLabel.Location = new System.Drawing.Point(17, 80);
            sourceLabel.Name = "sourceLabel";
            sourceLabel.Size = new System.Drawing.Size(64, 20);
            sourceLabel.TabIndex = 13;
            sourceLabel.Text = "Source:";
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Source", true));
            this.sourceTextBox.Location = new System.Drawing.Point(150, 77);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(200, 26);
            this.sourceTextBox.TabIndex = 14;
            // 
            // timestampLabel
            // 
            timestampLabel.AutoSize = true;
            timestampLabel.Location = new System.Drawing.Point(17, 51);
            timestampLabel.Name = "timestampLabel";
            timestampLabel.Size = new System.Drawing.Size(91, 20);
            timestampLabel.TabIndex = 15;
            timestampLabel.Text = "Timestamp:";
            // 
            // timestampTextBox
            // 
            this.timestampTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Timestamp", true));
            this.timestampTextBox.Location = new System.Drawing.Point(150, 48);
            this.timestampTextBox.Name = "timestampTextBox";
            this.timestampTextBox.Size = new System.Drawing.Size(200, 26);
            this.timestampTextBox.TabIndex = 16;
            // 
            // frmLogDetailViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 558);
            this.Controls.Add(timestampLabel);
            this.Controls.Add(this.timestampTextBox);
            this.Controls.Add(assemblyNameLabel);
            this.Controls.Add(this.assemblyNameTextBox);
            this.Controls.Add(classNameLabel);
            this.Controls.Add(this.classNameTextBox);
            this.Controls.Add(detailsLabel);
            this.Controls.Add(this.detailsTextBox);
            this.Controls.Add(messageLabel);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(methodNameLabel);
            this.Controls.Add(this.methodNameTextBox);
            this.Controls.Add(severityLabel);
            this.Controls.Add(this.severityTextBox);
            this.Controls.Add(sourceLabel);
            this.Controls.Add(this.sourceTextBox);
            this.Controls.Add(this.richLogEntryBindingNavigator);
            this.Name = "frmLogDetailViewer";
            this.Text = "frmLogDetailViewer";
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingNavigator)).EndInit();
            this.richLogEntryBindingNavigator.ResumeLayout(false);
            this.richLogEntryBindingNavigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource richLogEntryBindingSource;
        private System.Windows.Forms.BindingNavigator richLogEntryBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton richLogEntryBindingNavigatorSaveItem;
        private System.Windows.Forms.TextBox assemblyNameTextBox;
        private System.Windows.Forms.TextBox classNameTextBox;
        private System.Windows.Forms.TextBox detailsTextBox;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.TextBox methodNameTextBox;
        private System.Windows.Forms.TextBox severityTextBox;
        private System.Windows.Forms.TextBox sourceTextBox;
        private System.Windows.Forms.TextBox timestampTextBox;
    }
}
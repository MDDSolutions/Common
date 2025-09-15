namespace MDDWinForms
{
    partial class frmLogDetailViewer
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.BindingSource richLogEntryBindingSource;
        private System.Windows.Forms.BindingNavigator richLogEntryBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton richLogEntryBindingNavigatorSaveItem;

        // New: TableLayoutPanel for dynamic fields
        private System.Windows.Forms.TableLayoutPanel tlpFields;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.TextBox txtDetails;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogDetailViewer));
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

            this.tlpFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblDetails = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.TextBox();

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
                this.richLogEntryBindingNavigatorSaveItem
            });
            this.richLogEntryBindingNavigator.Location = new System.Drawing.Point(0, 520);
            this.richLogEntryBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.richLogEntryBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.richLogEntryBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.richLogEntryBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.richLogEntryBindingNavigator.Name = "richLogEntryBindingNavigator";
            this.richLogEntryBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.richLogEntryBindingNavigator.Size = new System.Drawing.Size(814, 38);
            this.richLogEntryBindingNavigator.TabIndex = 0;
            this.richLogEntryBindingNavigator.Text = "bindingNavigator1";
            this.richLogEntryBindingNavigator.Dock = System.Windows.Forms.DockStyle.Bottom;
            // 
            // binding navigator items setup
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            //
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            //
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 35);
            //
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 31);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            //
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(54, 30);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            //
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 35);
            //
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            //
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            //
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 35);
            //
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            //
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            //
            this.richLogEntryBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.richLogEntryBindingNavigatorSaveItem.Enabled = false;
            this.richLogEntryBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("richLogEntryBindingNavigatorSaveItem.Image")));
            this.richLogEntryBindingNavigatorSaveItem.Name = "richLogEntryBindingNavigatorSaveItem";
            this.richLogEntryBindingNavigatorSaveItem.Size = new System.Drawing.Size(34, 33);
            this.richLogEntryBindingNavigatorSaveItem.Text = "Save Data";
            // 
            // tlpFields
            // 
            this.tlpFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpFields.AutoScroll = true;
            this.tlpFields.AutoSize = true;
            this.tlpFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpFields.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            this.tlpFields.ColumnCount = 4;
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFields.Location = new System.Drawing.Point(12, 12);
            this.tlpFields.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFields.Name = "tlpFields";
            this.tlpFields.RowCount = 0;
            this.tlpFields.RowStyles.Clear();
            this.tlpFields.Size = new System.Drawing.Size(790, 150);
            this.tlpFields.TabIndex = 1;
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(12, 172);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(52, 20);
            this.lblDetails.TabIndex = 2;
            this.lblDetails.Text = "Details:";
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(110, 168);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(692, 344);
            this.txtDetails.TabIndex = 3;
            this.txtDetails.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.richLogEntryBindingSource, "Details", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            // 
            // frmLogDetailViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 558);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.tlpFields);
            this.Controls.Add(this.richLogEntryBindingNavigator);
            this.Name = "frmLogDetailViewer";
            this.Text = "Log Detail Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.richLogEntryBindingNavigator)).EndInit();
            this.richLogEntryBindingNavigator.ResumeLayout(false);
            this.richLogEntryBindingNavigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

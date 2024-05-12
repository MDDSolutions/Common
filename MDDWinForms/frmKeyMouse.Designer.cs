namespace MDDWinForms
{
    partial class frmKeyMouse
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
            this.txtInactiveTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrMain = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txtMouseLocation = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMouseColor = new System.Windows.Forms.TextBox();
            this.chkKeyDown = new System.Windows.Forms.CheckBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.chkKeyUp = new System.Windows.Forms.CheckBox();
            this.chkKeyPress = new System.Windows.Forms.CheckBox();
            this.chkMouseDoubleClick = new System.Windows.Forms.CheckBox();
            this.chkMouseClick = new System.Windows.Forms.CheckBox();
            this.chkMouseMove = new System.Windows.Forms.CheckBox();
            this.chkMouseUp = new System.Windows.Forms.CheckBox();
            this.chkMouseDown = new System.Windows.Forms.CheckBox();
            this.chkHandleThings = new System.Windows.Forms.CheckBox();
            this.txtFindWindow = new System.Windows.Forms.TextBox();
            this.lblFindWindow = new System.Windows.Forms.Label();
            this.txtWindowPtr = new System.Windows.Forms.TextBox();
            this.txtClickX = new System.Windows.Forms.TextBox();
            this.txtClickY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInactiveTime
            // 
            this.txtInactiveTime.Location = new System.Drawing.Point(121, 10);
            this.txtInactiveTime.Name = "txtInactiveTime";
            this.txtInactiveTime.Size = new System.Drawing.Size(100, 20);
            this.txtInactiveTime.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Inactive Time:";
            // 
            // tmrMain
            // 
            this.tmrMain.Enabled = true;
            this.tmrMain.Interval = 200;
            this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mouse Location:";
            // 
            // txtMouseLocation
            // 
            this.txtMouseLocation.Location = new System.Drawing.Point(121, 36);
            this.txtMouseLocation.Name = "txtMouseLocation";
            this.txtMouseLocation.Size = new System.Drawing.Size(100, 20);
            this.txtMouseLocation.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Color at Mouse:";
            // 
            // txtMouseColor
            // 
            this.txtMouseColor.Location = new System.Drawing.Point(121, 62);
            this.txtMouseColor.Name = "txtMouseColor";
            this.txtMouseColor.Size = new System.Drawing.Size(100, 20);
            this.txtMouseColor.TabIndex = 6;
            // 
            // chkKeyDown
            // 
            this.chkKeyDown.AutoSize = true;
            this.chkKeyDown.Location = new System.Drawing.Point(254, 13);
            this.chkKeyDown.Name = "chkKeyDown";
            this.chkKeyDown.Size = new System.Drawing.Size(72, 17);
            this.chkKeyDown.TabIndex = 8;
            this.chkKeyDown.Text = "KeyDown";
            this.chkKeyDown.UseVisualStyleBackColor = true;
            this.chkKeyDown.CheckedChanged += new System.EventHandler(this.chkKeyDown_CheckedChanged);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(3, 88);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(940, 360);
            this.txtOutput.TabIndex = 9;
            // 
            // chkKeyUp
            // 
            this.chkKeyUp.AutoSize = true;
            this.chkKeyUp.Location = new System.Drawing.Point(254, 35);
            this.chkKeyUp.Name = "chkKeyUp";
            this.chkKeyUp.Size = new System.Drawing.Size(58, 17);
            this.chkKeyUp.TabIndex = 10;
            this.chkKeyUp.Text = "KeyUp";
            this.chkKeyUp.UseVisualStyleBackColor = true;
            this.chkKeyUp.CheckedChanged += new System.EventHandler(this.chkKeyUp_CheckedChanged);
            // 
            // chkKeyPress
            // 
            this.chkKeyPress.AutoSize = true;
            this.chkKeyPress.Location = new System.Drawing.Point(254, 58);
            this.chkKeyPress.Name = "chkKeyPress";
            this.chkKeyPress.Size = new System.Drawing.Size(70, 17);
            this.chkKeyPress.TabIndex = 11;
            this.chkKeyPress.Text = "KeyPress";
            this.chkKeyPress.UseVisualStyleBackColor = true;
            this.chkKeyPress.CheckedChanged += new System.EventHandler(this.chkKeyPress_CheckedChanged);
            // 
            // chkMouseDoubleClick
            // 
            this.chkMouseDoubleClick.AutoSize = true;
            this.chkMouseDoubleClick.Location = new System.Drawing.Point(332, 58);
            this.chkMouseDoubleClick.Name = "chkMouseDoubleClick";
            this.chkMouseDoubleClick.Size = new System.Drawing.Size(115, 17);
            this.chkMouseDoubleClick.TabIndex = 14;
            this.chkMouseDoubleClick.Text = "MouseDoubleClick";
            this.chkMouseDoubleClick.UseVisualStyleBackColor = true;
            this.chkMouseDoubleClick.CheckedChanged += new System.EventHandler(this.chkMouseDoubleClick_CheckedChanged);
            // 
            // chkMouseClick
            // 
            this.chkMouseClick.AutoSize = true;
            this.chkMouseClick.Location = new System.Drawing.Point(332, 35);
            this.chkMouseClick.Name = "chkMouseClick";
            this.chkMouseClick.Size = new System.Drawing.Size(81, 17);
            this.chkMouseClick.TabIndex = 13;
            this.chkMouseClick.Text = "MouseClick";
            this.chkMouseClick.UseVisualStyleBackColor = true;
            this.chkMouseClick.CheckedChanged += new System.EventHandler(this.chkMouseClick_CheckedChanged);
            // 
            // chkMouseMove
            // 
            this.chkMouseMove.AutoSize = true;
            this.chkMouseMove.Location = new System.Drawing.Point(332, 13);
            this.chkMouseMove.Name = "chkMouseMove";
            this.chkMouseMove.Size = new System.Drawing.Size(85, 17);
            this.chkMouseMove.TabIndex = 12;
            this.chkMouseMove.Text = "MouseMove";
            this.chkMouseMove.UseVisualStyleBackColor = true;
            this.chkMouseMove.CheckedChanged += new System.EventHandler(this.chkMouseMove_CheckedChanged);
            // 
            // chkMouseUp
            // 
            this.chkMouseUp.AutoSize = true;
            this.chkMouseUp.Location = new System.Drawing.Point(452, 34);
            this.chkMouseUp.Name = "chkMouseUp";
            this.chkMouseUp.Size = new System.Drawing.Size(72, 17);
            this.chkMouseUp.TabIndex = 16;
            this.chkMouseUp.Text = "MouseUp";
            this.chkMouseUp.UseVisualStyleBackColor = true;
            this.chkMouseUp.CheckedChanged += new System.EventHandler(this.chkMouseUp_CheckedChanged);
            // 
            // chkMouseDown
            // 
            this.chkMouseDown.AutoSize = true;
            this.chkMouseDown.Location = new System.Drawing.Point(452, 12);
            this.chkMouseDown.Name = "chkMouseDown";
            this.chkMouseDown.Size = new System.Drawing.Size(86, 17);
            this.chkMouseDown.TabIndex = 15;
            this.chkMouseDown.Text = "MouseDown";
            this.chkMouseDown.UseVisualStyleBackColor = true;
            this.chkMouseDown.CheckedChanged += new System.EventHandler(this.chkMouseDown_CheckedChanged);
            // 
            // chkHandleThings
            // 
            this.chkHandleThings.AutoSize = true;
            this.chkHandleThings.Location = new System.Drawing.Point(544, 12);
            this.chkHandleThings.Name = "chkHandleThings";
            this.chkHandleThings.Size = new System.Drawing.Size(95, 17);
            this.chkHandleThings.TabIndex = 17;
            this.chkHandleThings.Text = "Handle Things";
            this.chkHandleThings.UseVisualStyleBackColor = true;
            // 
            // txtFindWindow
            // 
            this.txtFindWindow.Location = new System.Drawing.Point(531, 56);
            this.txtFindWindow.Name = "txtFindWindow";
            this.txtFindWindow.Size = new System.Drawing.Size(100, 20);
            this.txtFindWindow.TabIndex = 18;
            this.txtFindWindow.TextChanged += new System.EventHandler(this.txtFindWindow_TextChanged);
            // 
            // lblFindWindow
            // 
            this.lblFindWindow.AutoSize = true;
            this.lblFindWindow.Location = new System.Drawing.Point(453, 59);
            this.lblFindWindow.Name = "lblFindWindow";
            this.lblFindWindow.Size = new System.Drawing.Size(72, 13);
            this.lblFindWindow.TabIndex = 19;
            this.lblFindWindow.Text = "Find Window:";
            // 
            // txtWindowPtr
            // 
            this.txtWindowPtr.Location = new System.Drawing.Point(637, 56);
            this.txtWindowPtr.Name = "txtWindowPtr";
            this.txtWindowPtr.Size = new System.Drawing.Size(48, 20);
            this.txtWindowPtr.TabIndex = 20;
            // 
            // txtClickX
            // 
            this.txtClickX.Location = new System.Drawing.Point(703, 11);
            this.txtClickX.Name = "txtClickX";
            this.txtClickX.Size = new System.Drawing.Size(30, 20);
            this.txtClickX.TabIndex = 21;
            // 
            // txtClickY
            // 
            this.txtClickY.Location = new System.Drawing.Point(739, 11);
            this.txtClickY.Name = "txtClickY";
            this.txtClickY.Size = new System.Drawing.Size(30, 20);
            this.txtClickY.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(652, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Click at:";
            // 
            // frmKeyMouse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtClickY);
            this.Controls.Add(this.txtClickX);
            this.Controls.Add(this.txtWindowPtr);
            this.Controls.Add(this.lblFindWindow);
            this.Controls.Add(this.txtFindWindow);
            this.Controls.Add(this.chkHandleThings);
            this.Controls.Add(this.chkMouseUp);
            this.Controls.Add(this.chkMouseDown);
            this.Controls.Add(this.chkMouseDoubleClick);
            this.Controls.Add(this.chkMouseClick);
            this.Controls.Add(this.chkMouseMove);
            this.Controls.Add(this.chkKeyPress);
            this.Controls.Add(this.chkKeyUp);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.chkKeyDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMouseColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMouseLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInactiveTime);
            this.Name = "frmKeyMouse";
            this.Text = "frmKeyMouse";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInactiveTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrMain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMouseLocation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMouseColor;
        private System.Windows.Forms.CheckBox chkKeyDown;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.CheckBox chkKeyUp;
        private System.Windows.Forms.CheckBox chkKeyPress;
        private System.Windows.Forms.CheckBox chkMouseDoubleClick;
        private System.Windows.Forms.CheckBox chkMouseClick;
        private System.Windows.Forms.CheckBox chkMouseMove;
        private System.Windows.Forms.CheckBox chkMouseUp;
        private System.Windows.Forms.CheckBox chkMouseDown;
        private System.Windows.Forms.CheckBox chkHandleThings;
        private System.Windows.Forms.TextBox txtFindWindow;
        private System.Windows.Forms.Label lblFindWindow;
        private System.Windows.Forms.TextBox txtWindowPtr;
        private System.Windows.Forms.TextBox txtClickX;
        private System.Windows.Forms.TextBox txtClickY;
        private System.Windows.Forms.Label label3;
    }
}
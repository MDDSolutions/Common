using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public partial class ctlDropDownControl : UserControl
    {
        public enum eDockSide
        {
            Left,
            Right
        }
        public enum eDropState
        {
            Closed,
            Closing,
            Dropping,
            Dropped
        }
        DropDownContainer dropContainer;
        Control _dropDownItem;
        bool closedWhileInControl;
        private Size storedSize;
        protected eDropState DropState { get; private set; }
        private string _Text;
        public override string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                Invalidate();
            }
        }
        public ctlDropDownControl()
        {
            InitializeComponent();
            storedSize = Size;
            BackColor = Color.White;
            TabStop = true;
            Text = Name;
        }
        public void InitializeDropDown(Control dropDownItem)
        {
            if (_dropDownItem != null)
                throw new Exception("The drop down item has already been implemented!");
            _DesignView = false;
            _defaultDropDownSize = dropDownItem.Size;
            if (Controls.Contains(dropDownItem))
                Controls.Remove(dropDownItem);
            DropState = eDropState.Closed;
            Size = _AnchorSize;
            _AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 21, _AnchorSize.Height - 4);
            //removes the dropDown item from the controls list so it 
            //won't be seen until the drop-down window is active

            _dropDownItem = dropDownItem;
        }
        private Size _defaultDropDownSize;
        public Size DropDownSize
        {
            // The hosted control is resized to the popup's client area. Keeping the
            // requested size separately prevents the window border from shrinking
            // the popup again each time it is opened.
            get { return _defaultDropDownSize; }
            set
            {
                _defaultDropDownSize = value;
                if (_dropDownItem != null)
                    _dropDownItem.Size = value;
            }
        }


        private Size _AnchorSize = new Size(121, 21);
        public Size AnchorSize
        {
            get { return _AnchorSize; }
            set
            {
                _AnchorSize = value;
                this.Invalidate();
            }
        }
        public eDockSide DockSide { get; set; }
        private bool _DesignView = true;
        [DefaultValue(false)]
        protected bool DesignView
        {
            get { return _DesignView; }
            set
            {
                if (_DesignView == value) return;

                _DesignView = value;
                if (_DesignView)
                {
                    Size = storedSize;
                }
                else
                {
                    storedSize = Size;
                    Size = _AnchorSize;
                }

            }
        }
        public event EventHandler PropertyChanged;
        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(null, null);
        }

        private Rectangle _AnchorClientBounds;
        public Rectangle AnchorClientBounds
        {
            get { return _AnchorClientBounds; }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_DesignView)
                storedSize = Size;
            _AnchorSize.Width = Width;
            if (!_DesignView)
            {
                _AnchorSize.Height = Height;
                _AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 21, _AnchorSize.Height - 4);
            }
        }
        protected bool mousePressed;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mousePressed = true;
            OpenDropDown();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mousePressed = false;
            this.Invalidate();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            Keys modifiers = keyData & Keys.Modifiers;
            if (DropState == eDropState.Closed &&
                ((keyCode == Keys.F4 && modifiers == Keys.None) ||
                 (keyCode == Keys.Enter && modifiers == Keys.None) ||
                 (keyCode == Keys.Space && modifiers == Keys.None) ||
                 (keyCode == Keys.Down && modifiers == Keys.Alt)))
            {
                OpenDropDown();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        internal virtual bool ProcessDropDownKey(Keys keyData)
        {
            return false;
        }
        protected virtual bool CanDrop
        {
            get
            {
                if (dropContainer != null)
                    return false;

                if (dropContainer == null && closedWhileInControl)
                {
                    closedWhileInControl = false;
                    return false;
                }

                return !closedWhileInControl;
            }
        }
        public event EventHandler Dropping;
        public void OpenDropDown()
        {
            if (_dropDownItem == null)
                throw new NotImplementedException("The drop down item has not been initialized!  Use the InitializeDropDown() method to do so.");

            if (!CanDrop) return;

            dropContainer = new DropDownContainer(_dropDownItem, this, GetDropDownLocation(), DropDownSize);
            dropContainer.DropStateChange += new DropDownContainer.DropWindowArgs(dropContainer_DropStateChange);
            dropContainer.FormClosed += new FormClosedEventHandler(dropContainer_Closed);
            DropState = eDropState.Dropping;
            Form owner = FindForm();
            if (owner == null)
                dropContainer.Show();
            else
                dropContainer.Show(owner);
            Dropping?.Invoke(this, null);
            DropState = eDropState.Dropped;
            Invalidate();
            //PrintSizes(this);
            //PrintSizes(dropContainer);
            dropContainer.Loading = false;
        }
        public void PrintSizes(Control cntrl)
        {
            Console.WriteLine($"{cntrl.Name}: {cntrl.Size} @ {cntrl.Location}");
            foreach (Control c in cntrl.Controls)
            {
                PrintSizes(c);
            }
        }
        public void CloseDropDown()
        {
            if (dropContainer != null)
            {
                DropState = eDropState.Closing;
                dropContainer.Freeze = false;
                dropContainer.Close();
            }
        }
        void dropContainer_DropStateChange(eDropState state)
        {
            DropState = state;
        }
        void dropContainer_Closed(object sender, FormClosedEventArgs e)
        {
            if (!dropContainer.IsDisposed)
            {
                dropContainer.DropStateChange -= dropContainer_DropStateChange;
                dropContainer.FormClosed -= dropContainer_Closed;
                dropContainer.Dispose();
            }
            dropContainer = null;
            closedWhileInControl = (RectangleToScreen(ClientRectangle).Contains(Cursor.Position));
            DropState = eDropState.Closed;
            Invalidate();
        }
        protected virtual Point GetDropDownLocation()
        {
            if (DockSide == eDockSide.Left)
                return Parent.PointToScreen(new Point(Bounds.X, Bounds.Bottom));
            return Parent.PointToScreen(new Point(Bounds.Right - DropDownSize.Width, Bounds.Bottom));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(new Point(0, 0), _AnchorSize), getState());
            ComboBoxRenderer.DrawDropDownButton(e.Graphics,
               new Rectangle(_AnchorSize.Width - 19, 2, 18, _AnchorSize.Height - 4),
               getState());
            using (Brush b = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(b, AnchorClientBounds);
            }
            TextRenderer.DrawText(e.Graphics, _Text, Font, AnchorClientBounds, ForeColor, TextFormatFlags.WordEllipsis);
        }
        private System.Windows.Forms.VisualStyles.ComboBoxState getState()
        {
            if (mousePressed || dropContainer != null)
                return System.Windows.Forms.VisualStyles.ComboBoxState.Pressed;
            else
                return System.Windows.Forms.VisualStyles.ComboBoxState.Normal;
        }
        public void FreezeDropDown(bool remainVisible)
        {
            if (dropContainer != null)
            {
                dropContainer.Freeze = true;
                if (!remainVisible)
                    dropContainer.Visible = false;
            }
        }
        public void UnFreezeDropDown()
        {
            if (dropContainer != null)
            {
                dropContainer.Freeze = false;
                if (!dropContainer.Visible)
                    dropContainer.Visible = true;
            }
        }
        internal sealed class DropDownContainer : Form, IMessageFilter
        {
            public bool Freeze;
            public bool Loading = true;
            public ctlDropDownControl DropDownControl { get; set; }
            public DropDownContainer(Control dropDownItem, ctlDropDownControl dropDownControl, Point location, Size clientSize)
            {
                Name = "frmDropDown";
                DropDownControl = dropDownControl;
                ControlBox = false;
                MinimizeBox = false;
                MaximizeBox = false;
                Text = String.Empty;
                FormBorderStyle = FormBorderStyle.SizableToolWindow;
                StartPosition = FormStartPosition.Manual;
                ShowInTaskbar = false;
                Location = location;
                ClientSize = clientSize;
                dropDownItem.Dock = DockStyle.Fill;
                Controls.Add(dropDownItem);
                Application.AddMessageFilter(this);
            }
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                if (DropDownControl.ProcessDropDownKey(keyData))
                    return true;

                return base.ProcessCmdKey(ref msg, keyData);
            }
            public bool PreFilterMessage(ref Message m)
            {
                if (!Freeze && Visible && (ActiveForm == null || !ActiveForm.Equals(this)))
                {
                    OnDropStateChange(eDropState.Closing);
                    Close();
                }
                return false;
            }
            public delegate void DropWindowArgs(eDropState state);
            public event DropWindowArgs DropStateChange;
            public void OnDropStateChange(eDropState state)
            {
                DropStateChange?.Invoke(state);
            }
            //protected override void OnPaint(PaintEventArgs e)
            //{
            //    base.OnPaint(e);
            //    e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1));
            //}
            protected override void OnClosing(CancelEventArgs e)
            {
                Application.RemoveMessageFilter(this);
                Controls.RemoveAt(0); //prevent the control from being disposed
                base.OnClosing(e);
            }
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                if (!Loading)
                {
                    DropDownControl.DropDownSize = ClientSize;
                    //DropDownControl.PrintSizes(this);
                }
            }
        }
    }
}

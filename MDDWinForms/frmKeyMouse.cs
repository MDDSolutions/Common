using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmKeyMouse : Form
    {
        public frmKeyMouse()
        {
            InitializeComponent();
        }
        private IntPtr CurForegroundWindow = IntPtr.Zero;
        //private int msgsec;
        private int msg;

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            tmrMain.Enabled = false;
            try
            {
                var inactive = KeyMouse.GetInactiveTime();
                //if (inactive.TotalSeconds < 5) msgsec = 5;
                if (inactive.TotalSeconds >= 3 && !string.IsNullOrWhiteSpace(txtClickX.Text) && !string.IsNullOrWhiteSpace(txtClickY.Text))
                {
                    if (int.TryParse(txtClickX.Text, out int X) && int.TryParse(txtClickY.Text, out int Y))
                    {
                        txtClickX.Text = null;
                        txtClickY.Text = null;
                        KeyMouse.DoMouseClick(X, Y);
                    }
                }
                txtInactiveTime.Text = inactive.ToString(@"h\:mm\:ss");
                if (!chkMouseMove.Checked) txtMouseLocation.Text = $"(tmr){Cursor.Position.X} , {Cursor.Position.Y}";
                Color clr = KeyMouse.GetPixelColor(Cursor.Position.X, Cursor.Position.Y);
                txtMouseColor.BackColor = clr;
                txtMouseColor.Text = clr.ToArgb().ToString();
                var ptr = KeyMouse.GetForegroundWindow();
                if (Wptr == IntPtr.Zero && ProcessID != 0 && inactive.TotalSeconds >= 1)
                {
                    Wptr = KeyMouse.SetForegroundWindow(default, ProcessID);
                }
                if (Wptr != IntPtr.Zero)
                {
                    //msgsec += 1;
                    msg += 1;
                    txtOutput.AppendText("THIS DOES NOT WORK RIGHT NOW");
                    //if (!KeyMouse.SendKeysExt($"message {msg}\r\n", ProcessID, 1000, true, Wptr))
                    //    txtOutput.AppendText("SendKeys returned false\r\n");
                }
                if (ptr != CurForegroundWindow)
                {
                    CurForegroundWindow = ptr;
                    var p = KeyMouse.GetProcessByHandle(ptr);
                    txtOutput.AppendText($"{ptr}:{p.Id}:{p.ProcessName}\r\n");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                tmrMain.Enabled = true;
            }


        }
        private void chkKeyDown_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeyDown.Checked)
                KeyMouse.KeyDown += KeyMouse_KeyDown;
            else
                KeyMouse.KeyDown -= KeyMouse_KeyDown;
        }
        private void KeyMouse_KeyDown(object sender, KMKeyboardEventArgs e)
        {
            DisplayKey(e, "KeyDown");
        }
        private void chkKeyUp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeyUp.Checked)
                KeyMouse.KeyUp += KeyMouse_KeyUp;
            else
                KeyMouse.KeyUp -= KeyMouse_KeyUp;
        }
        private void KeyMouse_KeyUp(object sender, KMKeyboardEventArgs e)
        {
            DisplayKey(e, "KeyUp");
        }
        private void chkKeyPress_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeyPress.Checked)
                KeyMouse.KeyPress += KeyMouse_KeyPress;
            else
                KeyMouse.KeyPress -= KeyMouse_KeyPress;

        }
        private void KeyMouse_KeyPress(object sender, KMKeyboardEventArgs e)
        {
            DisplayKey(e,"KeyPress");
        }
        private void DisplayKey(KMKeyboardEventArgs e, string action)
        {
            var keyData = (Keys)e.VirtualKeyCode;
            var mod = "";
            if (KeyMouse.ShiftPressed) mod += "Shift";
            if (KeyMouse.AltPressed) mod += "Alt";
            if (KeyMouse.CtrlPressed) mod += "Cntrl";
            if (mod != "") mod += "-";
            txtOutput.AppendText($"{action}: {mod}{keyData}\r\n");
            if (chkHandleThings.Checked) e.Handled = true;
        }
        private void chkMouseMove_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseMove.Checked)
                KeyMouse.MouseMove += KeyMouse_MouseMove;
            else
                KeyMouse.MouseMove -= KeyMouse_MouseMove;
        }
        private void KeyMouse_MouseMove(object sender, KMMouseEventArgs e)
        {
            txtMouseLocation.Text = $"(km){e.Location.X} , {e.Location.Y}";
        }
        private void chkMouseClick_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseClick.Checked)
                KeyMouse.MouseClick += KeyMouse_MouseClick;
            else
                KeyMouse.MouseClick -= KeyMouse_MouseClick;
        }
        private void KeyMouse_MouseClick(object sender, KMMouseEventArgs e)
        {
            DisplayMouse(e, "MouseClick");
            if (e.Button == KMMouseButton.Right)
            {
                txtClickX.Text = e.Location.X.ToString();
                txtClickY.Text = e.Location.Y.ToString();
            }
        }
        private void chkMouseDoubleClick_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseDoubleClick.Checked)
                KeyMouse.MouseDoubleClick += KeyMouse_MouseDoubleClick;
            else
                KeyMouse.MouseDoubleClick -= KeyMouse_MouseDoubleClick;
        }
        private void KeyMouse_MouseDoubleClick(object sender, KMMouseEventArgs e)
        {
            DisplayMouse(e, "MouseDoubleClick");
        }
        private void chkMouseDown_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseDown.Checked)
                KeyMouse.MouseDown += KeyMouse_MouseDown;
            else
                KeyMouse.MouseDown -= KeyMouse_MouseDown;
        }
        private void KeyMouse_MouseDown(object sender, KMMouseEventArgs e)
        {
            DisplayMouse(e, "MouseDown");
        }
        private void chkMouseUp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMouseUp.Checked)
                KeyMouse.MouseUp += KeyMouse_MouseUp;
            else
                KeyMouse.MouseUp -= KeyMouse_MouseUp;
        }
        private void KeyMouse_MouseUp(object sender, KMMouseEventArgs e)
        {
            DisplayMouse(e, "MouseUp");
        }
        private void DisplayMouse(KMMouseEventArgs e, string action)
        {
            var mod = "";
            if (KeyMouse.ShiftPressed) mod += "Shift";
            if (KeyMouse.AltPressed) mod += "Alt";
            if (KeyMouse.CtrlPressed) mod += "Cntrl";
            if (mod != "") mod += "-";
            txtOutput.AppendText($"{action}: {mod}{e.Location.X}, {e.Location.Y}\r\n");
            if (chkHandleThings.Checked) e.Handled = true;
        }
        private IntPtr Wptr = IntPtr.Zero;
        private int ProcessID;
        private void txtFindWindow_TextChanged(object sender, EventArgs e)
        {
            var procs = KeyMouse.FindProcess(txtFindWindow.Text);
            if (procs.Count == 1)
            {
                ProcessID = procs[0].Id;
                //Wptr = KeyMouse.FindWindowByCaption(IntPtr.Zero, procs[0].MainWindowTitle);
                txtWindowPtr.Text = ProcessID.ToString();
            }
            else
            {
                Wptr = IntPtr.Zero;
                ProcessID = 0;
                txtWindowPtr.Text = "0";
            }
        }
    }
}

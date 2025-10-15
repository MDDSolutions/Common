using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MDDWinForms
{
    [DefaultEvent("ClipboardChanged")]
    public class ClipboardMonitor : Control
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        public ClipboardMonitor()
        {
            this.BackColor = Color.Red;
            this.Visible = false;
            if (!AddClipboardFormatListener(this.Handle))
                throw new Win32Exception("Failed to register clipboard listener.");
        }

        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;
        public event EventHandler ShutDown;
        public void SendShutDown()
        {
            ShutDown?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            RemoveClipboardFormatListener(this.Handle);
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
            }
            base.WndProc(ref m);
        }

        private void OnClipboardChanged()
        {
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(iData));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }

    public class ClipboardChangedEventArgs : EventArgs
    {
        public IDataObject DataObject { get; }

        public ClipboardChangedEventArgs(IDataObject dataObject)
        {
            DataObject = dataObject;
        }
    }
}
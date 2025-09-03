using System;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class TextBoxSearch : TextBox
    {
        public int SearchDelay { get; set; } = 500;

        private Timer searchTimer;
        private bool _isInitializing = true;
        public event EventHandler<string> UserSearch;

        public TextBoxSearch()
        {
            searchTimer = new Timer();
            searchTimer.Interval = SearchDelay;
            searchTimer.Tick += SearchTimer_Tick;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _isInitializing = false;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (_isInitializing)
                return;

            searchTimer.Stop();
            searchTimer.Interval = SearchDelay;
            searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            string searchText = this.Text.Trim();
            UserSearch?.Invoke(this, searchText);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter && !_isInitializing)
            {
                searchTimer.Stop();
                string searchText = this.Text.Trim();
                UserSearch?.Invoke(this, searchText);
                e.Handled = true; // Optional: prevents default beep
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && searchTimer != null)
            {
                searchTimer.Dispose();
                searchTimer = null;
            }
            base.Dispose(disposing);
        }
    }
}

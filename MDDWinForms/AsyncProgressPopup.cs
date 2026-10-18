using MDDFoundation;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    /// <summary>A movable, modeless progress window whose lifetime follows an awaited operation.</summary>
    public sealed class AsyncProgressPopup : Form
    {
        private readonly Label message;
        private readonly ProgressBar progressBar;

        private AsyncProgressPopup(string title)
        {
            Text = title;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 110);
            Padding = new Padding(12);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ControlBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            message = new Label { Dock = DockStyle.Fill, Text = "Starting...", AutoEllipsis = true };
            progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                Maximum = 1000,
                Style = ProgressBarStyle.Marquee
            };
            Controls.Add(message);
            Controls.Add(progressBar);
        }

        /// <summary>Call and await on the UI thread. Runs the operation on a worker thread,
        /// marshals reports to the UI, and closes/disposes the popup on success, cancellation or
        /// failure. Exceptions propagate to the caller; the owner remains usable throughout.</summary>
        public static async Task RunAsync(IWin32Window owner, string title,
            BasicProgressDelegate operation, CancellationToken token = default)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (SynchronizationContext.Current is not WindowsFormsSynchronizationContext)
                throw new InvalidOperationException("Show async progress from the WinForms UI thread.");

            using (var popup = new AsyncProgressPopup(title))
            {
                var progress = new Progress<BasicProgress>(popup.UpdateProgress);
                try
                {
                    if (owner == null) popup.Show();
                    else popup.Show(owner);
                    await Task.Run(() => operation(token, progress), token);
                }
                finally
                {
                    if (!popup.IsDisposed) popup.Close();
                }
            }
        }

        private void UpdateProgress(BasicProgress progress)
        {
            // Reports can already be queued on the UI context when the operation finishes.
            if (IsDisposed || Disposing || progress == null) return;
            message.Text = progress.Message ?? string.Empty;
            if (progress.TotalCount <= 0)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                return;
            }
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = (int)Math.Max(0, Math.Min(1000,
                1000.0 * progress.CurrentCount / progress.TotalCount));
        }
    }
}

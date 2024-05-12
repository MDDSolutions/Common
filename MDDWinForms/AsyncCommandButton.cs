using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace MDDWinForms
{
    public class AsyncCommandButton : Button
    {
        public bool AllowCancellation { get; set; } = true;
        private string cancellationtext = null;
        public string CancellationText 
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cancellationtext) && cancellationtext != "<default>") return cancellationtext;
                if (AllowCancellation) return "Cancel";
                return "Running";
            }
            set
            {
                cancellationtext = value;
            }
        }
        private string cancellingtext = null;
        public string CancellingText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cancellingtext) && cancellingtext != "<default>") return cancellingtext;
                if (AllowCancellation) return "Cancelling";
                return "Running";
            }
            set
            {
                cancellingtext = value;
            }
        }
        public int UpdateIntervalMS { get; set; } = 1000;
        public bool ShowElapsed { get; set; } = true;
        private CancellationTokenSource cts = null;
        private string buttontext = null;
        private Timer timer = null;
        private Stopwatch stopwatch = null;
        private long lastms = 0;
        private bool flip = true;

        public event Func<CancellationToken, Task> CommandAction;
        public event EventHandler<CancellationTokenSource> CancellationRequested;

        protected async override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (cts == null)
            {
                if (CommandAction == null) throw new NullReferenceException("AsyncCommandButton: CommandAction event is not handled");
                try
                {
                    buttontext = Text;
                    Text = $"{CancellationText}...";
                    flip = true;
                    timer = new Timer();
                    timer.Interval = 100;
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    stopwatch = Stopwatch.StartNew();
                    lastms = 0;
                    cts = new CancellationTokenSource();
                    await CommandAction(cts.Token);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Text = buttontext;
                    cts = null;
                    if (stopwatch != null)
                    {
                        stopwatch.Stop();
                        stopwatch = null;
                    }
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }
                }
            }
            else if (!cts.IsCancellationRequested)
            {
                if (CancellationRequested != null)
                {
                    CancellationRequested.Invoke(this, cts);
                }
                else
                {
                    if (AllowCancellation)
                        cts.Cancel();
                    else
                        MessageBox.Show("Process is already running - cancellation is not available");
                }
                if (cts != null && cts.IsCancellationRequested) Text = $"{CancellingText}...";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds >= lastms + UpdateIntervalMS)
            {
                string txt = cts.IsCancellationRequested ? CancellingText : CancellingText;

                if (ShowElapsed)
                {
                    Text = $"{txt} ({stopwatch.Elapsed.TotalSeconds:N0})...";
                }
                else
                {
                    if (flip)
                        Text = $"...{txt}";
                    else
                        Text = $"{txt}...";
                    flip = !flip;
                }
                lastms = lastms + UpdateIntervalMS;
            }
        }

        //public Func<CancellationToken,Task> CommandAction { get; set; }
    }
}

using System;
using System.Windows.Forms;

namespace MDDWinForms.Menus
{
    public enum MenuLifetimeMode { ExitApplication, CloseLauncher }

    /// <summary>Use CloseLauncher for a launcher of independent processes. No process termination is performed.</summary>
    public sealed class MenuLauncherApplicationContext : ApplicationContext
    {
        private readonly Form launcher;
        private readonly MenuLifetimeMode mode;
        private bool exitPending;
        public MenuLauncherApplicationContext(Form launcher, MenuLifetimeMode mode) : base(launcher)
        {
            this.launcher = launcher ?? throw new ArgumentNullException(nameof(launcher));
            this.mode = mode;
            launcher.FormClosing += LauncherClosing;
        }
        private void LauncherClosing(object sender, FormClosingEventArgs e)
        {
            if (mode != MenuLifetimeMode.ExitApplication || e.CloseReason != CloseReason.UserClosing || e.Cancel) return;
            e.Cancel = true;
            if (exitPending) return;
            exitPending = true;
            // Exit raises FormClosing on all windows and honors cancellation before closing any of them.
            launcher.BeginInvoke(new Action(() => { try { Application.Exit(); } finally { exitPending = false; } }));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) launcher.FormClosing -= LauncherClosing;
            base.Dispose(disposing);
        }
    }
}

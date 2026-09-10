using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MDDWinForms
{
    /// <summary>Reads the colour of any pixel on screen, via the desktop device context.</summary>
    public static class ScreenPixel
    {
        [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")] private static extern uint GetPixel(IntPtr hdc, int x, int y);

        public static Color ColorAt(int x, int y)
        {
            var hdc = GetDC(IntPtr.Zero);
            try
            {
                var pixel = GetPixel(hdc, x, y);
                return Color.FromArgb((int)(pixel & 0xFF), (int)(pixel & 0xFF00) >> 8, (int)(pixel & 0xFF0000) >> 16);
            }
            finally { ReleaseDC(IntPtr.Zero, hdc); }
        }
        public static Color ColorAt(Point position) => ColorAt(position.X, position.Y);
    }

    /// <summary>
    /// Shows where the mouse is and what colour is under it, which is the only part of the old
    /// VideoTitles frmAppInteraction2 still worth keeping. Useful when working out screen-scraping
    /// coordinates. Polling stops while the control is not visible.
    /// </summary>
    public class ctlScreenPixelProbe : UserControl
    {
        private readonly TextBox position = new TextBox { ReadOnly = true, Width = 120 };
        private readonly TextBox swatch = new TextBox { ReadOnly = true, Width = 40 };
        private readonly TextBox argb = new TextBox { ReadOnly = true, Width = 120 };
        private readonly Timer timer = new Timer { Interval = 100 };

        /// <summary>How often the readout updates, in milliseconds.</summary>
        public int PollInterval
        {
            get => timer.Interval;
            set => timer.Interval = Math.Max(10, value);
        }

        public ctlScreenPixelProbe()
        {
            Font = new Font("Segoe UI", 10);
            Padding = new Padding(8);
            var layout = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
            layout.Controls.Add(new Label { Text = "Mouse", AutoSize = true, Margin = new Padding(3, 7, 3, 3) });
            layout.Controls.Add(position);
            layout.Controls.Add(swatch);
            layout.Controls.Add(argb);
            Controls.Add(layout);
            // ControlForm sizes its window from the control, so a menu-launched control needs its own.
            Size = new Size(Math.Max(360, layout.PreferredSize.Width + Padding.Horizontal),
                            layout.PreferredSize.Height + Padding.Vertical);
            timer.Tick += (s, e) => Sample();
            VisibleChanged += (s, e) => timer.Enabled = Visible;
        }

        private void Sample()
        {
            var at = Cursor.Position;
            position.Text = at.X + "," + at.Y;
            var colour = ScreenPixel.ColorAt(at);
            swatch.BackColor = colour;
            argb.Text = colour.ToArgb().ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { timer.Stop(); timer.Dispose(); }
            base.Dispose(disposing);
        }
    }
}

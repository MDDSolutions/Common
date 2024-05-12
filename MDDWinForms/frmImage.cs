using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmImage : Form
    {
        string ImageFolder;
        List<string> Images;
        int CurImage;

        public frmImage()
        {
            InitializeComponent();
        }
        public void SetPosition (Point point, ScreenUsage screenusage, WindowPositions windowposition, bool useworkingarea = false)
        {
            Screen screen = null;
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Bounds.Contains(point))
                {
                    screen = s;
                    break;
                }
            }
            if (screen != null)
            {
                Rectangle r;
                if (useworkingarea)
                    r = MDDForms.GetConfigRectangle(screen.WorkingArea, windowposition, screenusage, 0, point);
                else
                    r = MDDForms.GetConfigRectangle(screen.Bounds, windowposition, screenusage, 0, point);

                WindowState = FormWindowState.Normal;
                StartPosition = FormStartPosition.Manual;
                BringToFront();
                FormBorderStyle = FormBorderStyle.None;
                Location = r.Location;
                Width = r.Width;
                Height = r.Height;
            }
        }
        public void SetImage(string filename)
        {
            pbx.Image = Image.FromFile(filename);
        }
        public void SetSlideshow (string folder,int intervalsec)
        {
            var di = new DirectoryInfo(folder);
            ImageFolder = di.FullName;

            Images = new List<string>();

            foreach (var item in di.GetFiles())
            {
                if (item.Extension == ".jpg")
                {
                    Images.Add(item.Name);
                }
            }

            if (Images.Count > 0)
            {
                CurImage = 0;
                SetImage(Path.Combine(ImageFolder, Images[CurImage]));
                tmr.Interval = intervalsec * 1000;
                tmr.Enabled = true;
            }

        }
        private void tmr_Tick(object sender, EventArgs e)
        {
            if (Images != null && Images.Count > 1)
            {
                CurImage = CurImage + 1;
                if (CurImage >= Images.Count) CurImage = 0;
                SetImage(Path.Combine(ImageFolder, Images[CurImage]));
            }
        }

        private void pbx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Close?", "Question", MessageBoxButtons.OKCancel) == DialogResult.OK)
                Close();
        }
    }
}

using MDDFoundation;
using MDDWinForms;
using System;
using System.Diagnostics;
//using System.DirectoryServices;
using System.Drawing;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmImageEdit : Form
    {
        private byte[] OrigImg;
        private Image Img = null;
        private byte[] editedimage = null;
        public byte[] EditedImage => editedimage;
        private int HorizontalOffset;
        private static Rectangle savedpos = new Rectangle(0, 0, 0, 0);
        private bool isinitializing = true;
        private static decimal dpifactor = 0;
        private static decimal xoffset = 0;
        private Size ScaleTo = default;
        public static decimal DefaultSystemDPI { get; set; } = 144;

        public frmImageEdit()
        {
            InitializeComponent();
        }

        public frmImageEdit(byte[] inImgBytes, Size scaleto = default)
        {
            ScaleTo = scaleto;
            isinitializing = true;
            InitializeComponent();

            if (dpifactor == 0)
            {
                var hProcess = Process.GetCurrentProcess().Handle;
                DpiAwareness dpiawareness;
                MDDForms.GetProcessDpiAwareness(hProcess, out dpiawareness);
                Foundation.Log($"frmImageEdit: DPIAwareness is {dpiawareness}");
                if (dpiawareness == DpiAwareness.Unaware)
                {
                    int dpi = 144;
                    Foundation.Log($"frmImageEdit: DPIForWindow is {dpi}");
                    dpifactor = DefaultSystemDPI / 96.0m;
                }
                else
                {
                    dpifactor = 1;
                }
            }

            decimal tdpifacor;
            if (!decimal.TryParse(txtDPIFactor.Text, out tdpifacor)) tdpifacor = 0;

            if (tdpifacor != dpifactor)
            {
                txtDPIFactor.Text = dpifactor.ToString();
            }

            Img = MDDForms.ByteArrayToImage(inImgBytes);
            OrigImg = inImgBytes;
            HorizontalOffset = 0;
            SetImage();
        }

        private void btnGetFromClipboard_Click(object sender, EventArgs e)
        {
            if (Img != null) Img.Dispose();
            Img = Clipboard.GetImage();
            if (Img == null)
            {
                MessageBox.Show("The clipboard does not appear to contain an image");
            }
            else
            {
                OrigImg = MDDForms.ToByteArray(Img);
                HorizontalOffset = 0;
                SetImage();
            }
        }

        private void SetImage()
        {
            try
            {
                var bm = new Bitmap(Img.Width, Img.Height);
                using (var canvas = Graphics.FromImage(bm))
                {
                    canvas.DrawImage(Img, 0, 0);
                    var r = GetRectangle();
                    if (chkCrop.Checked) canvas.DrawRectangle(Pens.Black, r);
                    if (pbx.Image != null) pbx.Image.Dispose();
                    pbx.Image = bm;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            HorizontalOffset -= int.Parse(txtIncrement.Text);
            SetImage();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            HorizontalOffset += int.Parse(txtIncrement.Text);
            SetImage();
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (chkCrop.Checked)
            {
                var bmp = new Bitmap(Img);
                var CropImg = bmp.Clone(GetRectangle(), bmp.PixelFormat);
                Img.Dispose();
                Img = CropImg;
                chkCrop.Checked = false;
            }
        }

        private Rectangle GetRectangle()
        {
            if (!theRectangle.IsEmpty) return theRectangle;

            int width = Img.Height / 200 * 170;
            int left = (Img.Width - width) / 2;
            return new Rectangle(left + HorizontalOffset, 0, width, Img.Height);
        }

        private void chkCrop_CheckedChanged(object sender, EventArgs e)
        {
            SetImage();
            if (!chkCrop.Checked)
            {
                theRectangle = new Rectangle(0, 0, 0, 0);
                BackupScreenRectangle = new Rectangle(0, 0, 0, 0);
            }
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            if (!isDrag && !BackupScreenRectangle.IsEmpty)
                ControlPaint.DrawReversibleFrame(DashedRect, this.BackColor, FrameStyle.Dashed);
        }

        private bool isDrag = false;
        private Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
        private Rectangle BackupScreenRectangle = new Rectangle(0, 0, 0, 0);
        private Point startPoint;
        private void pbx_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
            }
            var control = (Control)sender;
            startPoint = control.PointToScreen(new Point(e.X, e.Y));
        }

        private Rectangle DashedRect = new Rectangle(0, 0, 0, 0);
        private void pbx_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                ControlPaint.DrawReversibleFrame(DashedRect, this.BackColor, FrameStyle.Dashed);

                var endPoint = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
                int width = endPoint.X - startPoint.X;
                int height = endPoint.Y - startPoint.Y;
                theRectangle = new Rectangle(startPoint.X, startPoint.Y, width, height);

                DashedRect = new Rectangle(
                    (int)(theRectangle.X * dpifactor + xoffset),
                    (int)(theRectangle.Y * dpifactor),
                    (int)(theRectangle.Width * dpifactor),
                    (int)(theRectangle.Height * dpifactor)
                );

                ControlPaint.DrawReversibleFrame(DashedRect, this.BackColor, FrameStyle.Dashed);
            }
        }

        private void pbx_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
            DashedRect = new Rectangle(0, 0, 0, 0);

            if (theRectangle.Width < 0)
            {
                theRectangle = new Rectangle(theRectangle.X + theRectangle.Width, theRectangle.Y, -theRectangle.Width, theRectangle.Height);
            }
            if (theRectangle.Height < 0)
            {
                theRectangle = new Rectangle(theRectangle.X, theRectangle.Y + theRectangle.Height, theRectangle.Width, -theRectangle.Height);
            }

            if (theRectangle.Height > 50 && theRectangle.Width > 50)
            {
                BackupScreenRectangle = new Rectangle(theRectangle.X, theRectangle.Y, theRectangle.Width, theRectangle.Height);

                while ((double)theRectangle.Width / theRectangle.Height > 0.85)
                {
                    theRectangle = new Rectangle(theRectangle.Left, theRectangle.Top - 1, theRectangle.Width, theRectangle.Height + 2);
                }
                while ((double)theRectangle.Width / theRectangle.Height < 0.85)
                {
                    theRectangle = new Rectangle(theRectangle.Left - 1, theRectangle.Top, theRectangle.Width + 2, theRectangle.Height);
                }

                theRectangle = pbx.RectangleToClient(theRectangle);

                double ratio, xOff, yOff;
                if ((double)pbx.Width / pbx.Height < (double)pbx.Image.Width / pbx.Image.Height)
                {
                    ratio = (double)pbx.Image.Width / pbx.Width;
                    xOff = 0;
                    yOff = (pbx.Height - (pbx.Image.Height / ratio)) / 2;
                }
                else
                {
                    ratio = (double)pbx.Image.Height / pbx.Height;
                    xOff = (pbx.Width - (pbx.Image.Width / ratio)) / 2;
                    yOff = 0;
                }

                theRectangle = new Rectangle(
                    (int)((theRectangle.Left - xOff) * ratio),
                    (int)((theRectangle.Top - yOff) * ratio),
                    (int)(theRectangle.Width * ratio),
                    (int)(theRectangle.Height * ratio)
                );

                if (theRectangle.Top + theRectangle.Height > pbx.Image.Height)
                {
                    theRectangle = new Rectangle(theRectangle.Left, pbx.Image.Height - theRectangle.Height, theRectangle.Width, theRectangle.Height);
                }
                if (theRectangle.Left + theRectangle.Width > pbx.Image.Width)
                {
                    theRectangle = new Rectangle(pbx.Image.Width - theRectangle.Width, theRectangle.Top, theRectangle.Width, theRectangle.Height);
                }
                if (theRectangle.Top < 0)
                {
                    theRectangle = new Rectangle(theRectangle.Left, 0, theRectangle.Width, theRectangle.Height);
                }
                if (theRectangle.Left < 0)
                {
                    theRectangle = new Rectangle(0, theRectangle.Top, theRectangle.Width, theRectangle.Height);
                }
                if (theRectangle.Height > pbx.Image.Height)
                {
                    theRectangle = new Rectangle(theRectangle.Left, 0, theRectangle.Width, pbx.Image.Height);
                }
                if (theRectangle.Width > pbx.Image.Width)
                {
                    theRectangle = new Rectangle(0, theRectangle.Top, pbx.Image.Width, theRectangle.Height);
                }
                chkCrop.Checked = true;
            }
            else
            {
                theRectangle = new Rectangle(0, 0, 0, 0);
            }
        }

        private void frmImageEdit_Load(object sender, EventArgs e)
        {
            if (savedpos.Width == 0)
            {
                savedpos = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            }
            else
            {
                this.Location = new Point(savedpos.X, savedpos.Y);
                this.Width = savedpos.Width;
                this.Height = savedpos.Height;
            }
            isinitializing = false;
        }

        private void frmImageEdit_Move(object sender, EventArgs e)
        {
            if (!isinitializing)
            {
                savedpos = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            }
        }

        private void frmImageEdit_Resize(object sender, EventArgs e)
        {
            if (!isinitializing)
            {
                savedpos = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            }
        }

        private void frmImageEdit_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.Print(theRectangle.ToString());
        }

        private void txtDPIFactor_TextChanged(object sender, EventArgs e)
        {
            decimal tdpifacor;
            if (decimal.TryParse(txtDPIFactor.Text, out tdpifacor) && tdpifacor != dpifactor)
            {
                dpifactor = tdpifacor;
            }
        }

        private void txtXOffset_TextChanged(object sender, EventArgs e)
        {
            decimal txoffset;
            if (decimal.TryParse(txtXOffset.Text, out txoffset) && txoffset != xoffset)
            {
                xoffset = txoffset;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ScaleTo != default && (Img.Height > ScaleTo.Height || Img.Width > ScaleTo.Width))
                MDDForms.ResizeImage(ref Img, ScaleTo);
            editedimage = MDDForms.ToByteArray(Img);
        }
    }
}
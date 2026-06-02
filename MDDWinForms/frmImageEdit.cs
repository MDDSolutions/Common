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

            int width = Math.Min(Img.Width, (int)Math.Round(Img.Height * GetTargetAspectRatio()));
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
        private RectangleDragMode dragMode = RectangleDragMode.None;
        private RectangleMoveAxis moveAxis = RectangleMoveAxis.None;
        private bool moveAxisLocksOnFirstMove = false;
        private Point moveStartImagePoint;
        private Rectangle moveStartRectangle;

        private enum RectangleDragMode
        {
            None,
            Draw,
            Move
        }

        private enum RectangleMoveAxis
        {
            None,
            Horizontal,
            Vertical
        }

        private void pbx_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var control = (Control)sender;
            startPoint = control.PointToScreen(new Point(e.X, e.Y));

            if (CanMoveRectangleFromPoint(new Point(e.X, e.Y)))
            {
                isDrag = true;
                dragMode = RectangleDragMode.Move;
                moveAxis = RectangleMoveAxis.None;
                moveAxisLocksOnFirstMove = (ModifierKeys & Keys.Shift) == Keys.Shift;
                moveStartImagePoint = ClientPointToImagePoint(new Point(e.X, e.Y));
                moveStartRectangle = theRectangle;
                BackupScreenRectangle = new Rectangle(0, 0, 0, 0);
                pbx.Cursor = Cursors.SizeAll;
                return;
            }

            isDrag = true;
            dragMode = RectangleDragMode.Draw;
        }

        private Rectangle DashedRect = new Rectangle(0, 0, 0, 0);
        private void pbx_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrag)
            {
                pbx.Cursor = CanMoveRectangleFromPoint(new Point(e.X, e.Y)) ? Cursors.SizeAll : Cursors.Default;
                return;
            }

            if (dragMode == RectangleDragMode.Move)
            {
                MoveRectangleToPoint(new Point(e.X, e.Y));
                return;
            }

            if (dragMode == RectangleDragMode.Draw)
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

            if (dragMode == RectangleDragMode.Move)
            {
                dragMode = RectangleDragMode.None;
                moveAxis = RectangleMoveAxis.None;
                moveAxisLocksOnFirstMove = false;
                pbx.Cursor = CanMoveRectangleFromPoint(new Point(e.X, e.Y)) ? Cursors.SizeAll : Cursors.Default;
                return;
            }

            if (dragMode != RectangleDragMode.Draw)
            {
                dragMode = RectangleDragMode.None;
                return;
            }

            dragMode = RectangleDragMode.None;
            theRectangle = NormalizeRectangle(theRectangle);

            if (IsUsableSelection(theRectangle))
            {
                BackupScreenRectangle = new Rectangle(theRectangle.X, theRectangle.Y, theRectangle.Width, theRectangle.Height);

                var imageRectangle = ClipToImage(ScreenRectangleToImageRectangle(theRectangle));
                if (imageRectangle.Width > 0 && imageRectangle.Height > 0)
                {
                    theRectangle = ExpandToTargetAspectRatio(imageRectangle);
                    chkCrop.Checked = true;
                }
                else
                {
                    theRectangle = new Rectangle(0, 0, 0, 0);
                }
            }
            else
            {
                theRectangle = new Rectangle(0, 0, 0, 0);
            }
        }

        private bool CanMoveRectangleFromPoint(Point clientPoint)
        {
            if (!chkCrop.Checked || theRectangle.IsEmpty || pbx.Image == null)
                return false;

            return theRectangle.Contains(ClientPointToImagePoint(clientPoint));
        }

        private bool IsUsableSelection(Rectangle rectangle)
        {
            return rectangle.Width > 0 && rectangle.Height > 0 && Math.Max(rectangle.Width, rectangle.Height) > 50;
        }

        private Point ClientPointToImagePoint(Point clientPoint)
        {
            double ratio, xOff, yOff;
            GetPictureBoxImageScale(out ratio, out xOff, out yOff);

            return new Point(
                (int)Math.Round((clientPoint.X - xOff) * ratio),
                (int)Math.Round((clientPoint.Y - yOff) * ratio)
            );
        }

        private void MoveRectangleToPoint(Point clientPoint)
        {
            var imagePoint = ClientPointToImagePoint(clientPoint);
            int dx = imagePoint.X - moveStartImagePoint.X;
            int dy = imagePoint.Y - moveStartImagePoint.Y;

            if (moveAxisLocksOnFirstMove)
            {
                if (moveAxis == RectangleMoveAxis.None && (Math.Abs(dx) > 0 || Math.Abs(dy) > 0))
                {
                    moveAxis = Math.Abs(dx) >= Math.Abs(dy)
                        ? RectangleMoveAxis.Horizontal
                        : RectangleMoveAxis.Vertical;
                }

                if (moveAxis == RectangleMoveAxis.Horizontal)
                    dy = 0;
                else if (moveAxis == RectangleMoveAxis.Vertical)
                    dx = 0;
            }

            theRectangle = FitWithinImage(
                moveStartRectangle.Left + dx,
                moveStartRectangle.Top + dy,
                moveStartRectangle.Width,
                moveStartRectangle.Height
            );
            SetImage();
        }

        private double GetTargetAspectRatio()
        {
            if (ScaleTo != default && ScaleTo.Width > 0 && ScaleTo.Height > 0)
                return (double)ScaleTo.Width / ScaleTo.Height;

            return 0.85;
        }

        private Rectangle NormalizeRectangle(Rectangle rectangle)
        {
            int left = Math.Min(rectangle.Left, rectangle.Right);
            int top = Math.Min(rectangle.Top, rectangle.Bottom);
            int right = Math.Max(rectangle.Left, rectangle.Right);
            int bottom = Math.Max(rectangle.Top, rectangle.Bottom);

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        private Rectangle ScreenRectangleToImageRectangle(Rectangle screenRectangle)
        {
            var clientRectangle = pbx.RectangleToClient(screenRectangle);
            double ratio, xOff, yOff;
            GetPictureBoxImageScale(out ratio, out xOff, out yOff);

            int left = (int)Math.Floor((clientRectangle.Left - xOff) * ratio);
            int top = (int)Math.Floor((clientRectangle.Top - yOff) * ratio);
            int right = (int)Math.Ceiling((clientRectangle.Right - xOff) * ratio);
            int bottom = (int)Math.Ceiling((clientRectangle.Bottom - yOff) * ratio);

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        private void GetPictureBoxImageScale(out double ratio, out double xOff, out double yOff)
        {
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
        }

        private Rectangle ClipToImage(Rectangle rectangle)
        {
            int left = Math.Max(0, rectangle.Left);
            int top = Math.Max(0, rectangle.Top);
            int right = Math.Min(pbx.Image.Width, rectangle.Right);
            int bottom = Math.Min(pbx.Image.Height, rectangle.Bottom);

            if (right <= left || bottom <= top)
                return new Rectangle(0, 0, 0, 0);

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        private Rectangle ExpandToTargetAspectRatio(Rectangle rectangle)
        {
            double aspectRatio = GetTargetAspectRatio();
            double left = rectangle.Left;
            double top = rectangle.Top;
            double width = rectangle.Width;
            double height = rectangle.Height;
            double centerX = rectangle.Left + rectangle.Width / 2.0;
            double centerY = rectangle.Top + rectangle.Height / 2.0;

            if (width >= height)
            {
                height = Math.Min(width / aspectRatio, pbx.Image.Height);
                top = centerY - height / 2.0;
            }
            else
            {
                width = Math.Min(height * aspectRatio, pbx.Image.Width);
                left = centerX - width / 2.0;
            }

            return FitWithinImage(left, top, width, height);
        }

        private Rectangle FitWithinImage(double left, double top, double width, double height)
        {
            width = Math.Min(width, pbx.Image.Width);
            height = Math.Min(height, pbx.Image.Height);

            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (left + width > pbx.Image.Width) left = pbx.Image.Width - width;
            if (top + height > pbx.Image.Height) top = pbx.Image.Height - height;

            int finalWidth = Math.Max(1, (int)Math.Round(width));
            int finalHeight = Math.Max(1, (int)Math.Round(height));
            int finalLeft = Math.Max(0, (int)Math.Round(left));
            int finalTop = Math.Max(0, (int)Math.Round(top));

            if (finalLeft + finalWidth > pbx.Image.Width) finalLeft = pbx.Image.Width - finalWidth;
            if (finalTop + finalHeight > pbx.Image.Height) finalTop = pbx.Image.Height - finalHeight;

            return new Rectangle(finalLeft, finalTop, finalWidth, finalHeight);
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

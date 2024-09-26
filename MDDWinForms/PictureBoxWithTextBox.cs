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
    public partial class PictureBoxWithTextBox : UserControl
    {
        #region Initialization and State
        public PictureBoxWithTextBox()
        {
            InitializeComponent();

            Reset();
        }

        static PictureBoxWithTextBox()
        {
            var hProcess = Process.GetCurrentProcess().Handle;
            MDDForms.GetProcessDpiAwareness(hProcess, out DpiAwareness dpiAwareness);

            if (dpiAwareness == DpiAwareness.Unaware)
            {
                AppDefaultSize = new Size(250, 400);
                AppDefaultSizeLandscape = new Size(400, 250);
            }
            else
            {
                AppDefaultSize = new Size(600, 960);
                AppDefaultSizeLandscape = new Size(960, 600);
            }

            //This was in Program
            //AppDefaultSize = New Size(309 * 2, 492 * 2)
        }

        private int highlighthickness;
        private Color highlightcolor;
        private bool showtextbox;
        private bool sizetoimage;
        private bool selected;
        public virtual void Reset()
        {
            highlighthickness = 0;
            highlightcolor = Color.Yellow;
            showtextbox = true;
            sizetoimage = true;
            pbx.Image = null;
            txt.Text = null;
            selected = false;
            tsmSelect.Text = "Select";
            SelectionChanged = null;
            PictureBoxClicked = null;
            refreshall(AppDefaultSize);
        }
        #endregion


        #region Public Properties
        public int HighlightThickness 
        { 
            get => highlighthickness;
            set
            {
                if (highlighthickness != value)
                {
                    highlighthickness = value;
                    refreshall();
                }
            }
        }
        public Color HighlightColor 
        {   
            get => highlightcolor;
            set
            {
                if (highlightcolor != value)
                {
                    highlightcolor = value;
                    refreshall();
                }
            }
        }
        public Rectangle UsableSurface { get => new Rectangle(highlighthickness, highlighthickness, Width - (highlighthickness * 2), Height - (highlighthickness * 2)); }
        public bool ShowTextBox 
        { 
            get => showtextbox;
            set
            {
                if (showtextbox != value)
                {
                    showtextbox = value;
                    refreshall();
                }
            }
        }
        public bool SizeToImage
        {
            get => sizetoimage;
            set
            {
                if (sizetoimage != value)
                {
                    sizetoimage = value;
                    refreshall();
                }
            }
        }
        public bool Selected 
        { 
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    if (selected)
                    {
                        highlightcolor = Color.Yellow;
                        HighlightThickness = 5;
                        tsmSelect.Text = "DeSelect";
                    }
                    else
                    {
                        HighlightThickness = 0;
                        tsmSelect.Text = "Select";
                    }
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public static Size AppDefaultSize { get; set; }
        public static Size AppDefaultSizeLandscape { get; set; }
        #endregion


        #region Public and Virtual Methods (including some Event Handlers)
        public void SetImage(Image img, bool withresize = true)
        {
            sizetoimage = withresize;

            if (img == null) throw new ArgumentNullException("PictureBoxWithTextBox - image is null");
            if (withresize)
            {
                refreshall(pbx.Size);
                if (img.Size.Width > pbx.Size.Width || img.Size.Height > pbx.Size.Height)
                {
                    img = MDDForms.ResizeImage(img, pbx.Size);
                }
                if (img.Size.Width < AppDefaultSize.Width * 0.9 && img.Size.Height < AppDefaultSize.Height * 0.9)
                {
                    img = MDDForms.ResizeImage(img, AppDefaultSize);
                }
                if (img.Size.Width < pbx.Size.Width || img.Size.Height < pbx.Size.Height)
                {
                    refreshall(img.Size);
                }
            }
            pbx.Image = img;
        }
        public void SetText(string text, bool strict = true)
        {
            if (!string.IsNullOrWhiteSpace(text))
                ShowTextBox = true;
            else if (strict)
                ShowTextBox = false;
            txt.Text = text;
        }
        public virtual void CopyImage(object sender, EventArgs e)
        {
            Clipboard.SetImage(pbx.Image);
        }
        public virtual void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            //no default implementation - this is just to make it overridable
        }
        #endregion

        #region Public Events
        public event EventHandler SelectionChanged;
        public event EventHandler PictureBoxClicked;
        #endregion


        #region Private methods and Event Handlers
        private void refreshall(Size imgsize = default)
        {
            if (sizetoimage)
            {
                if (imgsize == default) imgsize = pbx.Image.Size;
                Size = new Size(imgsize.Width + highlighthickness * 2, imgsize.Height + highlighthickness * 2 + (showtextbox ? txt.Height : 0));
            }
            var us = UsableSurface;
            pbx.Location = us.Location;
            pbx.Width = us.Width;
            if (showtextbox)
            {
                pbx.Height = us.Height - txt.Height;
                txt.Visible = true;
                txt.Location = new Point(pbx.Left, pbx.Top + pbx.Height);
                txt.Width = us.Width;
            }
            else
            {
                pbx.Height = us.Height;
                txt.Visible= false;
            }
            if (highlighthickness > 0)
            {
                using (var gfx = CreateGraphics())
                {
                    var pen = new Pen(highlightcolor, highlighthickness);
                    var thickmid = Convert.ToSingle(highlighthickness) / 2;
                    gfx.DrawRectangle(pen, thickmid, thickmid, Width - highlighthickness, Height - highlighthickness);
                }
            }
        }
        private void tsmSelect_Click(object sender, EventArgs e)
        {
            Selected = !Selected;
        }
        private void pbx_Click(object sender, EventArgs e)
        {
            PictureBoxClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}

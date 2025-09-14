using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class TextBoxPlaceHolder : TextBox
    {
        public string PlaceHolderText { get; set; }
        override public string Text
        {
            get
            {
                if (base.Text == PlaceHolderText)
                    return null;
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        override protected void OnCreateControl()
        {
            base.OnCreateControl();
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = PlaceHolderText;
                this.ForeColor = System.Drawing.SystemColors.GrayText;
            }
        }
        protected override void OnGotFocus(EventArgs e)
        {
            if (base.Text == PlaceHolderText)
            {
                this.Text = "";
                this.ForeColor = System.Drawing.SystemColors.WindowText;
            }
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = PlaceHolderText;
                this.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            base.OnLostFocus(e);
        }
        private string _previousText = null;
        public string PreviousText => _previousText;
        protected override void OnEnter(EventArgs e)
        {
            _previousText = this.Text;
        }
    }
}

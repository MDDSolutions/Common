using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmInput : Form
    {
        public frmInput()
        {
            InitializeComponent();
        }
        public frmInput(string title, string label, string defaultvalue = null) : this()
        {
            this.Text = title;
            lblLabel.Text = label;
            txtValue.Text = defaultvalue;
        }
        public string Value { get { return txtValue.Text; } }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

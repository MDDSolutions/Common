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
    public partial class frmMultilineInput : Form
    {
        public frmMultilineInput()
        {
            InitializeComponent();
        }
        public frmMultilineInput(string titletext, string placeholder) : this()
        {
            this.Text = titletext;
            txtInput.Text = placeholder;
        }
        public string Value { get => txtInput.Text; }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}

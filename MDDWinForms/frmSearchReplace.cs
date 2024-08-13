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
    public partial class frmSearchReplace : Form
    {
        public frmSearchReplace()
        {
            InitializeComponent();
        }
        public frmSearchReplace(string search) : this()
        {
            txtSearch.Text = search;
        }
        public string SearchString { get => txtSearch.Text; }
        public string ReplaceString { get => txtReplace.Text; }
    }
}

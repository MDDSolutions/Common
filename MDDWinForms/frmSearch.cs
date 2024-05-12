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
    public partial class frmSearch : Form
    {
        public frmSearch()
        {
            InitializeComponent();
        }
        private DataGridViewRowUpdate datagridview = null;
        public frmSearch(DataGridViewRowUpdate dgv):this()
        {
            datagridview = dgv;
        }
        DateTime lastupdate = DateTime.MaxValue;
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lastupdate = DateTime.Now;
            await Task.Delay(300);
            var now = DateTime.Now;
            if (lastupdate.AddMilliseconds(290) <= now)
            {
                var srch = txtSearch.Text.Split(' ');

                var bs = datagridview.DataSource as BindingSource;

                if (bs != null)
                {
                    if (bs.SupportsFiltering)
                    {

                    }
                    else
                    {
                        MessageBox.Show("BindingSource does not support filtering");
                    }
                }
                else
                {
                    var dv = datagridview.DataSource as DataView;
                    dv.RowFilter = "FileName LIKE '%" + srch[0] + "%'";

                }


            }
        }
    }
}

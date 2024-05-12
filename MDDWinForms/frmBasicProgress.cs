using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataObjects
{
    public partial class frmBasicProgress : Form
    {
        public frmBasicProgress()
        {
            InitializeComponent();
        }

        public BasicProgressDelegate BasicProgressMethod { get; set; }

        CancellationTokenSource tokensource = null;
        private string tailmsg;
        private async void btnGo_Click(object sender, EventArgs e)
        {
            if (tokensource == null)
            {
                var progress = new Progress<BasicProgress>(a => txtOutput.Text = $"{a}{tailmsg}");
                tokensource = new CancellationTokenSource();
                btnGo.Text = "Cancel";
                txtOutput.Text = "Starting...";
                tailmsg = "...";
                await BasicProgressMethod(tokensource.Token, progress);
                btnGo.Text = "Go";
                if (tokensource.IsCancellationRequested)
                    tailmsg = " Cancelled";
                else
                    tailmsg = " Finished";
                tokensource = null;
                progress = null;
                txtOutput.Text = txtOutput.Text.Replace("...", tailmsg);
            }
            else
            {
                btnGo.Text = "Cancelling...";
                tokensource.Cancel();
            }

        }
    }
}

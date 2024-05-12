using FormsDataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDDDataAccess;
using System.Threading;

namespace FormsDataAccess
{
    public partial class frmEditor : Form
    {
        public event EventHandler DataSaved;
        public bool IsNewRecord { get; set; }
        public frmEditor()
        {
            InitializeComponent();
        }

        public string TitleName = "<dynamic>";
        public DBEntity data = null;
        public FormDirtyTracker formdirtytracker;

        public virtual async Task SaveData()
        {

                await data.Save(CancellationToken.None);
                foreach (Control ctrl in this.Controls)
                    foreach (Binding binding in ctrl.DataBindings)
                        binding.ReadValue();
                DataSaved?.Invoke(this, null);
                formdirtytracker.ReInitialize();

        }
        public virtual void frmEditor_Load(object sender, EventArgs e)
        {
            if (data == null)
                Text = TitleName;
            else
                Text = string.Format("{0} {1}",TitleName, data.ToString());
            formdirtytracker = new FormDirtyTracker(this);
            formdirtytracker.DirtyChanged += Formdirtytracker_DirtyChanged;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveData();
                    if (chkCloseOnSave.Checked)
                        Close();
                    }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void Formdirtytracker_DirtyChanged(object sender, bool e)
        {
            var outstr = string.Format("{0} {1}", TitleName, data.ToString());
            if (e) outstr = outstr + " *";
            if (Text != outstr) Text = outstr;
        }
    }
}

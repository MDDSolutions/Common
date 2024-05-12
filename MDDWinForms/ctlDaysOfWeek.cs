using MDDFoundation;
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
    [DefaultBindingProperty("Value")]

    public partial class ctlDaysOfWeek : UserControl
    {
        public event EventHandler<DayOfWeek> Activity;
        public ctlDaysOfWeek()
        {
            InitializeComponent();
        }
        public DaysOfWeek Value
        {
            get
            {
                var retval = new DaysOfWeek();
                foreach (var chk in Controls.OfType<CheckBox>())
                {
                    if (chk.Checked)
                    {
                        if (Enum.TryParse(chk.Name.Replace("chk", ""), out DayOfWeek dow))
                        {
                            retval = retval.AddValue(dow);
                        }
                    }
                }
                return retval;
            }
            set
            {
                if (!DesignMode && IsHandleCreated) UpdateControls(value);
            }
        }

        private void UpdateControls(DaysOfWeek value)
        {
            foreach (DayOfWeek dow in Enum.GetValues(typeof(DayOfWeek)))
            {
                var chk = Controls[$"chk{dow}"] as CheckBox;
                if (chk != null) chk.Checked = value == null ? false : value.HasValue(dow);
            }
        }

        private void chkMonday_CheckedChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse(((Control)sender).Name.Replace("chk", ""), out DayOfWeek dow))
                Activity?.Invoke(this,dow);
        }
    }
}

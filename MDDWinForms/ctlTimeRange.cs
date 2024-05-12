using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    [DefaultBindingProperty("Value")]
    public class ctlTimeRange : TextBox
    {
        public TimeRange Value
        {
            get
            {
                if (TimeRange.TryParse(Text, out TimeRange range))
                    return range;
                return null;
            }
            set
            {
                originalvalue = value?.ToString();
                Text = value?.ToString();
            }
        }
        private string originalvalue;
        protected override void OnValidating(CancelEventArgs e)
        {
            if (TimeRange.TryParse(Text, out TimeRange range)) 
                Text = range.ToString();
            else
            {
                MessageBox.Show("Invalid TimeRange - try something like '9:00 - 17:00'");
                e.Cancel = true;
            }    
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Text = originalvalue;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

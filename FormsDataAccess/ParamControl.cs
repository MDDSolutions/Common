using MDDDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public class ParamTextBox : TextBox
    {
        public string ConfigProperty { get; set; }
        protected override void OnValidated(EventArgs e)
        {
            DBParameter.SetParamValue(ConfigProperty ?? this.Name, this.Text);
            base.OnValidated(e);
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                var obj = DBParameter.GetParamValue(ConfigProperty ?? this.Name);
                if (obj != null)
                    this.Text = obj.ToString();
            }
        }
    }
    public class ParamCheckBox : CheckBox
    {
        public string ConfigProperty { get; set; }
        protected override void OnValidated(EventArgs e)
        {
            DBParameter.SetParamValue(ConfigProperty ?? this.Name, this.Checked);
            base.OnValidated(e);
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
                this.Checked = (bool)DBParameter.GetParamValue(ConfigProperty ?? this.Name);
        }
    }
}

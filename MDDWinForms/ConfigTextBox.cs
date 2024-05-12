using MDDFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class ConfigTextBox : TextBox
    {
        public string ConfigProperty { get; set; }
        protected override void OnValidated(EventArgs e)
        {
            ConfigControl.PropertySetValue(ConfigProperty ?? this.Name,this.Text);
            base.OnValidated(e);
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                var obj = ConfigControl.PropertyGetValue(ConfigProperty ?? this.Name);
                if (obj != null)
                {
                    this.Text = obj.ToString();
                    OnValidated(null);
                }
            }
        }
    }
    public class ConfigCheckBox : CheckBox
    {
        public string ConfigProperty { get; set; }
        protected override void OnValidated(EventArgs e)
        {
            ConfigControl.PropertySetValue(ConfigProperty ?? this.Name, this.Checked);
            base.OnValidated(e);
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                this.Checked = (bool)ConfigControl.PropertyGetValue(ConfigProperty ?? this.Name);
                OnValidated(null);
            }
        }
    }
    public class ConfigControl
    {
        public static void PropertySetValue(string name, object value)
        {
            var prop = GetProperty(name);
            var currentvalue = prop.GetValue(Configuration);
            if (currentvalue == null || !currentvalue.Equals(value))
            {
                if (prop.PropertyType.Name == "Int32")
                    prop.SetValue(Configuration, Convert.ToInt32(value));
                else
                    prop.SetValue(Configuration, value);
                Configuration.Save();
            }
        }
        public static object PropertyGetValue(string name)
        {
            var prop = GetProperty(name);
            return prop.GetValue(Configuration);
        }
        public static PropertyInfo GetProperty(string name)
        {
            if (Configuration != null)
            {
                var props = Configuration.GetType().GetProperties().Where(x => name.IndexOf(x.Name,StringComparison.OrdinalIgnoreCase) != -1).ToList();
                if (props.Count == 1)
                    return props[0];
                else if (props.Count == 0)
                    throw new Exception($"ConfigControl: ConfigProperty '{name}' does not exist in ConfigControl.Configuration");
                else
                {
                    props = Configuration.GetType().GetProperties().Where(x => name.Equals(x.Name,StringComparison.OrdinalIgnoreCase)).ToList();
                    if (props.Count == 1)
                        return props[0];
                    else
                        throw new Exception($"ConfigControl: Multiple possible matches for '{name}' in ConfigControl.Configuration");
                }
            }
            else
            {
                throw new Exception("ConfigControl.Configuration is null");
            }
        }
        public static CustomConfiguration Configuration { get; set; }
    }
}

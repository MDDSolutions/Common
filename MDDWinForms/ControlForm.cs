using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class ControlForm : Form, IWorkspaceWindow
    {
        public ControlForm()
        {
            InitializeComponent();
        }
        string ContainedControlTypeName;
        string ContainedAssemblyName;
        public Control ContainedControl { get; private set; }
        public ControlForm(Control ctl, string title = null) : this()
        {
            InitializeForm(ctl, title);
        }
        private void InitializeForm(Control ctl, string title)
        {
            ContainedControl = ctl;

            Name = $"ControlForm:{ctl.Name}";
            
            string displayText = null;
            if (ctl is IControlForm icf)
            {
                displayText = icf.DisplayText;
                icf.DisplayTextChanged += (s, e) => 
                {
                    if (!string.IsNullOrWhiteSpace(MDDForms.InstanceQualifier))
                        Text = $"{MDDForms.InstanceQualifier} {e}";
                    else
                        Text = e;
                };
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(title)) 
                    displayText = title;
                else
                    displayText = ctl.Name;
            }
            if (!string.IsNullOrWhiteSpace(MDDForms.InstanceQualifier)) 
                Text = $"{MDDForms.InstanceQualifier} {displayText}";
            else
                Text = displayText;

            Size = new Size(ctl.Width + 10, ctl.Height + 50);
            ctl.Dock = DockStyle.Fill;
            Controls.Add(ctl);
            ContainedControlTypeName = ctl.GetType().FullName;
            ContainedAssemblyName = ctl.GetType().Assembly.GetName().Name;

            var okButtonProp = ctl.GetType().GetProperty("OKButton", BindingFlags.Public | BindingFlags.Instance);
            if (okButtonProp != null)
            {
                var okButton = okButtonProp.GetValue(ctl) as Button;
                if (okButton != null)
                {
                    AcceptButton = okButton;
                }
            }
        }
        public bool IgnoreWorkspaceState => false;
        public void ApplyWorkspaceState(string state)
        {
            var bounds = Bounds;
            var parts = state.Split('|');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid workspace state format.");
            }

            ContainedControlTypeName = parts[1];
            ContainedAssemblyName = parts[2];


            Type controlType = null;
            if (!string.IsNullOrEmpty(ContainedAssemblyName))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == ContainedAssemblyName);
                if (assembly != null)
                    controlType = assembly.GetType(ContainedControlTypeName);
            }
            if (controlType == null)
            {
                // Fallback: search all loaded assemblies
                controlType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => typeof(Form).IsAssignableFrom(t) && t.FullName == ContainedControlTypeName);
            }
            if (controlType == null)
            {
                throw new InvalidOperationException($"Control type '{ContainedControlTypeName}' not found.");
            }
            var ctl = (Control)Activator.CreateInstance(controlType);

            InitializeForm(ctl, parts[0]);
            Bounds = bounds;
        }
        public string GetWorkspaceState()
        {
            return $"{Text}|{ContainedControlTypeName}|{ContainedAssemblyName}";
        }
    }
    public interface IControlForm
    {
        event EventHandler<string> DisplayTextChanged;
        string DisplayText { get; }
    }
}

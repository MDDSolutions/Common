using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MDDFoundation.Menus;
using MenuItem = MDDFoundation.Menus.MenuItem;

namespace MDDWinForms.Menus
{
    public sealed class WinFormsMenuDispatcher : IMenuDispatcher
    {
        public bool UseInstanceQualifier { get; set; } = true;
        // Optional escape hatches for legacy screens with dependencies or special commands.
        public Func<MenuItem, MenuLaunchMode, bool> TryLaunch { get; set; }
        public Func<Type, Control> CreateControl { get; set; }

        public void Launch(MenuItem item, MenuLaunchMode mode)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Kind != MenuItemKind.Action) throw new ArgumentException("Categories cannot be launched.");
            if (!Enum.IsDefined(typeof(MenuLaunchMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            if (mode == MenuLaunchMode.CreateNew && !item.AllowNewInstance && item.DefaultLaunchMode != mode)
                throw new InvalidOperationException("This item does not allow an additional instance.");
            if (TryLaunch?.Invoke(item, mode) == true) return;
            if (item.TargetKind == MenuTargetKind.Process)
            {
                if (mode != MenuLaunchMode.CreateNew)
                    throw new InvalidOperationException("External applications use CreateNew; instance coordination belongs to the target application.");
                using (var process = Process.Start(new ProcessStartInfo(item.ExecutablePath, item.Arguments ?? "") { UseShellExecute = true })) { }
                return;
            }
            var quickRun = item.TargetKind == MenuTargetKind.StoredProcedure;
            Type type;
            if (quickRun)
            {
                try { type = Type.GetType("FormsDataAccess.QuickRunControl, FormsDataAccess", true); }
                catch (Exception ex) when (ex is System.IO.FileNotFoundException || ex is System.IO.FileLoadException || ex is TypeLoadException)
                {
                    throw new InvalidOperationException("Stored-procedure menu items require FormsDataAccess and DBEngine. Add those application references and initialize DBEngine.Default before launching Quick Run. " + ex.Message);
                }
            }
            else type = ResolveType(item);
            if (!typeof(Control).IsAssignableFrom(type) || type.IsAbstract || type.ContainsGenericParameters)
                throw new InvalidOperationException($"'{type.FullName}' must be a concrete WinForms form or control.");
            Form window = null;
            if (mode == MenuLaunchMode.ActivateExistingOrCreate)
                window = Application.OpenForms.Cast<Form>().FirstOrDefault(f => !f.IsDisposed && !f.InvokeRequired &&
                    (f.GetType() == type || (f is ControlForm wrapper && wrapper.ContainedControl?.GetType() == type && (!quickRun || (wrapper.ContainedControl is IMenuTargetIdentity identity && string.Equals(identity.MenuTargetKey, item.ProcedureName, StringComparison.OrdinalIgnoreCase))))));
            if (window == null)
            {
                Control control = null;
                try
                {
                    control = quickRun ? (Control)Activator.CreateInstance(type, item)
                        : CreateControl?.Invoke(type) ?? (Control)Activator.CreateInstance(type);
                    window = control as Form ?? new ControlForm(control, item.Title, UseInstanceQualifier);
                    if (control is Form && UseInstanceQualifier && !string.IsNullOrWhiteSpace(MDDForms.InstanceQualifier))
                    {
                        var prefix = MDDForms.InstanceQualifier + " ";
                        if (!window.Text.StartsWith(prefix, StringComparison.Ordinal)) window.Text = prefix + window.Text;
                    }
                    window.Show();
                }
                catch (Exception ex)
                {
                    window?.Dispose();
                    if (window == null) control?.Dispose();
                    var cause = ex.GetBaseException();
                    if (quickRun && (cause is System.IO.FileNotFoundException || cause is System.IO.FileLoadException || cause is TypeLoadException || cause is MissingMethodException))
                        throw new InvalidOperationException("Quick Run requires compatible FormsDataAccess and DBEngine assemblies. Add/update both application references and initialize DBEngine.Default. " + cause.Message);
                    throw;
                }
            }
            else window.Show();
            if (window.WindowState == FormWindowState.Minimized) window.WindowState = FormWindowState.Normal;
            window.BringToFront();
            window.Activate();
        }

        private static Type ResolveType(MenuItem item)
        {
            var type = Type.GetType(item.TargetTypeName, false);
            if (type != null) return type;
            if (!string.IsNullOrWhiteSpace(item.AssemblyName))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == item.AssemblyName || a.FullName == item.AssemblyName)
                    ?? Assembly.Load(item.AssemblyName);
                return assembly.GetType(item.TargetTypeName, true);
            }
            var matches = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(item.TargetTypeName, false)).Where(t => t != null).ToArray();
            if (matches.Length != 1) throw new InvalidOperationException($"Cannot uniquely resolve '{item.TargetTypeName}'. Supply its assembly name.");
            return matches[0];
        }
    }
}

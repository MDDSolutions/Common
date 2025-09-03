using MDDFoundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class WorkspaceManager : CustomConfiguration
    {
        public override void ApplyDefaults()
        {
            ActiveMonitors = GetActiveMonitors();
        }        
        public List<MonitorInfo> ActiveMonitors { get; set; }
        public List<WorkspaceConfiguration> Workspaces { get; set; }
        public bool ValidateMonitorLayout()
        {
            var currentMonitors = GetActiveMonitors();

            if (ActiveMonitors == null || currentMonitors.Count != ActiveMonitors.Count)
                return false;

            // Compare bounds and working areas for each monitor
            var savedBounds = ActiveMonitors.Select(m => m.Bounds).OrderBy(r => r.X).ThenBy(r => r.Y).ToList();
            var currentBounds = currentMonitors.Select(m => m.Bounds).OrderBy(r => r.X).ThenBy(r => r.Y).ToList();

            var savedWorkingAreas = ActiveMonitors.Select(m => m.WorkingArea).OrderBy(r => r.X).ThenBy(r => r.Y).ToList();
            var currentWorkingAreas = currentMonitors.Select(m => m.WorkingArea).OrderBy(r => r.X).ThenBy(r => r.Y).ToList();

            for (int i = 0; i < savedBounds.Count; i++)
            {
                if (!savedBounds[i].Equals(currentBounds[i]) ||
                    !savedWorkingAreas[i].Equals(currentWorkingAreas[i]))
                {
                    return false;
                }
            }

            ActiveMonitors = currentMonitors;
            return true;
        }
        public void ApplyWorkspace(WorkspaceConfiguration ws)
        {
            var monitors = ActiveMonitors ?? GetActiveMonitors();
            var monitorAreas = monitors.Select(m => m.WorkingArea).ToList();
            var primaryMonitor = monitors.FirstOrDefault(m => m.IsPrimary) ?? monitors.FirstOrDefault();

            foreach (var windowState in ws.WindowStates)
            {
                var form = windowState.ToForm();
                if (form != null)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    var iconPoint = new Point(windowState.Bounds.X + 10, windowState.Bounds.Y + 10);
                    bool isIconVisible = monitorAreas.Any(area => area.Contains(iconPoint));
                    Rectangle targetBounds = windowState.Bounds;

                    if (!isIconVisible && primaryMonitor != null)
                    {
                        targetBounds = new Rectangle(
                            primaryMonitor.WorkingArea.X,
                            primaryMonitor.WorkingArea.Y,
                            Math.Min(windowState.Bounds.Width, primaryMonitor.WorkingArea.Width),
                            Math.Min(windowState.Bounds.Height, primaryMonitor.WorkingArea.Height)
                        );
                    }

                    // Validate bounds size
                    if (!IsReasonableWindowBounds(targetBounds, monitorAreas) && primaryMonitor != null)
                    {
                        targetBounds = new Rectangle(
                            primaryMonitor.WorkingArea.X,
                            primaryMonitor.WorkingArea.Y,
                            Math.Min(primaryMonitor.WorkingArea.Width, 800),
                            Math.Min(primaryMonitor.WorkingArea.Height, 600)
                        );
                    }

                    form.Bounds = targetBounds;
                    form.WindowState = windowState.IsMaximized ? FormWindowState.Maximized :
                                       windowState.IsMinimized ? FormWindowState.Minimized :
                                       FormWindowState.Normal;
                    if (form is IWorkspaceWindow workspaceWindow)
                    {
                        workspaceWindow.ApplyWorkspaceState(windowState.WorkspaceState);
                    }
                    form.ShowInstance();
                }
            }
        }        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("WorkspaceManager:");
            sb.AppendLine($"Active Monitors: {ActiveMonitors.Count}");
            foreach (var monitor in ActiveMonitors)
            {
                sb.AppendLine(monitor.ToString());
            }
            return sb.ToString();
        }        


        public static List<MonitorInfo> GetActiveMonitors()
        {
            var monitors = new List<MonitorInfo>();
            foreach (var screen in Screen.AllScreens)
            {
                monitors.Add(new MonitorInfo
                {
                    DeviceName = screen.DeviceName,
                    Bounds = screen.Bounds,
                    WorkingArea = screen.WorkingArea,
                    IsPrimary = screen.Primary
                });
            }
            return monitors;
        }
        public static WorkspaceConfiguration GetCurrentWorkspace()
        {
            var workspace = new WorkspaceConfiguration
            {
                Name = "Current Workspace",
                WindowStates = new List<WindowState>()
            };
            foreach (var form in Application.OpenForms.Cast<Form>())
            {
                IWorkspaceWindow workspaceWindow = form as IWorkspaceWindow;

                if (workspaceWindow != null && workspaceWindow.IgnoreWorkspaceState) continue;

                var type = form.GetType();
                workspace.WindowStates.Add(new WindowState
                {
                    WindowName = form.Name,
                    Bounds = form.Bounds,
                    IsMaximized = form.WindowState == FormWindowState.Maximized,
                    IsMinimized = form.WindowState == FormWindowState.Minimized,
                    WorkspaceState = workspaceWindow?.GetWorkspaceState() ?? string.Empty,
                    FormTypeName = type.FullName,
                    FormAssemblyName = type.Assembly.GetName().Name
                });
            }
            return workspace;
        }

        private static WorkspaceManager _instance;
        public static WorkspaceManager Instance         
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Load<WorkspaceManager>("WorkspaceManager.xml");
                }
                return _instance;
            }
        }

        private static bool IsReasonableWindowBounds(Rectangle bounds, List<Rectangle> monitorAreas)
        {
            const int MinWidth = 100;
            const int MinHeight = 50;

            if (bounds.Width < MinWidth || bounds.Height < MinHeight)
                return false;

            // Optionally, check against the largest monitor area
            var maxWidth = monitorAreas.Max(a => a.Width) + 100;
            var maxHeight = monitorAreas.Max(a => a.Height) + 100;

            if (bounds.Width > maxWidth || bounds.Height > maxHeight)
                return false;

            return true;
        }
    }
    public class MonitorInfo
    {
        public string DeviceName { get; set; }
        public Rectangle Bounds { get; set; }
        public Rectangle WorkingArea { get; set; }
        public bool IsPrimary { get; set; }
        public override string ToString()
        {
            return $"{DeviceName} - {Bounds} (WorkingArea: {WorkingArea}, Primary: {IsPrimary})";
        }
    }
    public class WindowState
    {
        public string WindowName { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsMinimized { get; set; }
        public string WorkspaceState { get; set; } // Serialized state of the workspace
        public string FormTypeName { get; set; }   // Fully qualified type name
        public string FormAssemblyName { get; set; } // Assembly name
        public Form ToForm()
        {
            Type formType = null;
            if (!string.IsNullOrEmpty(FormAssemblyName))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == FormAssemblyName);
                if (assembly != null)
                    formType = assembly.GetType(FormTypeName);
            }
            if (formType == null)
            {
                // Fallback: search all loaded assemblies
                formType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => typeof(Form).IsAssignableFrom(t) && t.FullName == FormTypeName);
            }
            return formType != null ? Activator.CreateInstance(formType) as Form : null;
        }
        public override string ToString()
        {
            return $"{WindowName} - {Bounds} (Maximized: {IsMaximized}, Minimized: {IsMinimized}, Type: {FormTypeName})";
        }
    }
    public class WorkspaceConfiguration
    {
        public string Name { get; set; }
        public List<WindowState> WindowStates { get; set; } = new List<WindowState>();
        override public string ToString()
        {
            var name = Name ?? "(unnamed)";
            var count = WindowStates != null ? WindowStates.Count : 0;
            return $"{name} - {count} WindowStates";
        }
    }
    public interface IWorkspaceWindow
    {
        bool IgnoreWorkspaceState { get; }
        string GetWorkspaceState();

        // Applies a previously saved state to the window
        void ApplyWorkspaceState(string state);
    }
}

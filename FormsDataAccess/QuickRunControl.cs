using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDDFoundation.Menus;
using MDDDataAccess;
using MDDDataAccess.Menus;
using MenuItem = MDDFoundation.Menus.MenuItem;

namespace FormsDataAccess
{
    public sealed class QuickRunControl : UserControl, IMenuTargetIdentity
    {
        private readonly MenuItem item;
        private readonly IQuickRunService service;
        private readonly DataGridView parameters = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
        private readonly TextBox output = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false };
        private readonly Button execute = new Button { Text = "Execute", AutoSize = true, Enabled = false };
        private readonly Button cancel = new Button { Text = "Cancel", AutoSize = true, Enabled = false };
        private readonly Button reload = new Button { Text = "Reload parameters", AutoSize = true };
        private readonly Label status = new Label { AutoSize = true, Padding = new Padding(8) };
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 100 };
        private readonly Stopwatch elapsed = new Stopwatch();
        private CancellationTokenSource operation;
        private QuickRunDefinition definition;
        private Form host;
        private bool changing;
        private string phase;
        public string MenuTargetKey => item.ProcedureName;
        public bool IsBusy => operation != null;

        public QuickRunControl(MenuItem item) : this(item, DefaultService()) { }
        private static IQuickRunService DefaultService()
        {
            if (!DBEngine.IsDefaultInitialized)
                throw new InvalidOperationException("Quick Run requires DBEngine and FormsDataAccess, with DBEngine.Default initialized for the target database.");
            return DBEngine.Default;
        }
        public QuickRunControl(MenuItem item, IQuickRunService service)
        {
            this.item = item ?? throw new ArgumentNullException(nameof(item));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrWhiteSpace(item.ProcedureName)) throw new ArgumentException("Quick Run requires a stored procedure name.");
            Name = "QuickRun";
            Size = new Size(950, 570);
            Font = new Font("Segoe UI", 10);
            parameters.Columns.Add(new DataGridViewTextBoxColumn { Name = "Parameter", ReadOnly = true });
            parameters.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", ReadOnly = true });
            parameters.Columns.Add(new DataGridViewTextBoxColumn { Name = "Value", FillWeight = 160 });
            parameters.Columns.Add(new DataGridViewComboBoxColumn { Name = "Source", DataSource = new[] { "Default", "Last used", "Custom" } });
            parameters.Columns.Add(new DataGridViewCheckBoxColumn { Name = "SaveDefault", HeaderText = "Save as default" });
            parameters.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Omit", HeaderText = "Use procedure default" });
            parameters.CurrentCellDirtyStateChanged += (s, e) => { if (parameters.IsCurrentCellDirty) parameters.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            parameters.CellValueChanged += ParameterChanged;
            var heading = new Label { Text = item.Title + " — " + item.ProcedureName + "\r\nEnter NULL for SQL NULL. Use procedure default omits that parameter from the call.", Dock = DockStyle.Top, Height = 58 };
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 };
            actions.Controls.AddRange(new Control[] { execute, cancel, reload, status });
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, Size = Size, SplitterDistance = 260 };
            split.Panel1.Controls.Add(parameters);
            split.Panel2.Controls.Add(output);
            Controls.Add(split); Controls.Add(actions); Controls.Add(heading);
            execute.Click += async (s, e) => await ExecuteAsync();
            reload.Click += async (s, e) => await ReloadAsync();
            cancel.Click += (s, e) => Cancel();
            timer.Tick += (s, e) => status.Text = phase + " (" + elapsed.Elapsed.TotalSeconds.ToString("N1") + " s)";
        }
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            host = FindForm();
            if (host != null) host.FormClosing += HostClosing;
            await ReloadAsync();
        }
        private void HostClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsBusy) return;
            e.Cancel = true;
            Cancel();
            Append("Waiting for cancellation to finish. Close this window again when it has stopped.");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (host != null) { host.FormClosing -= HostClosing; host = null; }
                operation?.Cancel();
                timer.Dispose();
            }
            base.Dispose(disposing);
        }
        public void Cancel()
        {
            if (operation == null) return;
            phase = "Cancelling"; cancel.Enabled = false; operation.Cancel();
        }
        private void ParameterChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (changing || e.RowIndex < 0) return;
            var row = parameters.Rows[e.RowIndex];
            var parameter = row.Tag as QuickRunParameter;
            if (parameter == null) return;
            changing = true;
            try
            {
                if (e.ColumnIndex == 3)
                {
                    var source = Convert.ToString(row.Cells[3].Value);
                    if (source != "Custom") row.Cells[2].Value = source == "Default" ? parameter.DefaultValue : parameter.LastValue;
                }
                else if (e.ColumnIndex == 2) row.Cells[3].Value = "Custom";
            }
            finally { changing = false; }
        }
        public async Task ReloadAsync()
        {
            if (IsBusy || IsDisposed) return;
            await PerformAsync("Loading parameters", async token =>
            {
                definition = null; parameters.Rows.Clear();
                var loaded = await service.LoadAsync(item.ProcedureName, token);
                if (IsDisposed) return;
                definition = loaded; parameters.Rows.Clear(); changing = true;
                try
                {
                    foreach (var parameter in loaded.Parameters)
                    {
                        int index = parameters.Rows.Add(parameter.Name, parameter.TypeDescription,
                            parameter.DefaultValue, "Default", false, false);
                        parameters.Rows[index].Tag = parameter;
                    }
                }
                finally { changing = false; }
            });
        }
        public async Task ExecuteAsync()
        {
            if (IsBusy || definition == null || IsDisposed) return;
            parameters.EndEdit();
            var values = parameters.Rows.Cast<DataGridViewRow>().Select(row => new QuickRunValue {
                Parameter = (QuickRunParameter)row.Tag, Value = Convert.ToString(row.Cells[2].Value),
                SaveAsDefault = Convert.ToBoolean(row.Cells[4].Value), UseProcedureDefault = Convert.ToBoolean(row.Cells[5].Value)
            }).ToList();
            output.Clear();
            await PerformAsync("Executing", async token =>
            {
                var progress = new Progress<string>(Append);
                await service.ExecuteAsync(definition, values, progress, token);
                if (IsDisposed) return;
                foreach (var value in values.Where(v => !v.UseProcedureDefault))
                {
                    value.Parameter.LastValue = value.Value;
                    if (value.SaveAsDefault) value.Parameter.DefaultValue = value.Value;
                }
            });
        }
        private async Task PerformAsync(string action, Func<CancellationToken, Task> work)
        {
            var current = new CancellationTokenSource(); operation = current;
            phase = action; execute.Enabled = reload.Enabled = parameters.Enabled = false; cancel.Enabled = true;
            elapsed.Restart(); timer.Start();
            string result;
            try { await work(current.Token); result = action == "Executing" ? "Complete" : "Parameters ready"; }
            catch (Exception ex) when (current.IsCancellationRequested && (ex is OperationCanceledException || ex is SqlException)) { result = "Cancelled"; }
            catch (Exception ex) { result = "Failed"; Append(ex.GetBaseException().Message); }
            finally
            {
                elapsed.Stop();
                if (!IsDisposed) timer.Stop();
                operation = null; current.Dispose();
            }
            if (IsDisposed) return;
            status.Text = result + " (" + elapsed.Elapsed.TotalSeconds.ToString("N1") + " s)";
            Append(status.Text);
            execute.Enabled = definition != null; reload.Enabled = parameters.Enabled = true; cancel.Enabled = false;
        }
        private void Append(string message)
        {
            if (!IsDisposed) output.AppendText(message + Environment.NewLine);
        }
    }
}

using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmLogViewer : Form
    {
        private RichLog _currentLog;
        private EventHandler<RichLogEntry> _currentHandler;
        private BindingList<RichLogEntry> _displayedEntries = new BindingList<RichLogEntry>();

        public frmLogViewer()
        {
            InitializeComponent();
            LoadActiveLogs();
            bsEntries.DataSource = _displayedEntries;
        }
        private void LoadActiveLogs()
        {
            cboLogs.Items.Clear();
            cboLogs.DisplayMember = "LogName";
            foreach (var log in RichLog.ActiveLogs)
            {
                cboLogs.Items.Add(log);
            }

            if (cboLogs.Items.Count > 0)
                cboLogs.SelectedIndex = 0;
        }
        private void cboLogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = cboLogs.SelectedItem as RichLog;
            if (selected == _currentLog)
                return;

            DeactivateFilter();

            _currentLog = selected;
        }
        private DateTime since = DateTime.MinValue;
        private Func<RichLogEntry, bool> MakeFilterFunc()
        {
            return e =>
            {
                if (e.Timestamp < since)
                    return false;
                if (e.Severity < severity)
                    return false;
                if (!string.IsNullOrEmpty(txtSource.Text) && !e.Source.Contains(txtSource.Text))
                    return false;
                if (!string.IsNullOrEmpty(txtAssembly.Text) && !e.AssemblyName.Contains(txtAssembly.Text))
                    return false;
                if (!string.IsNullOrEmpty(txtClass.Text) && !e.ClassName.Contains(txtClass.Text))
                    return false;
                if (!string.IsNullOrEmpty(txtMethod.Text) && !e.MethodName.Contains(txtMethod.Text))
                    return false;
                if (!string.IsNullOrEmpty(txtKeyword.Text) && !e.Message.Contains(txtKeyword.Text))
                    return false;

                return true;
            };
        }
        private void ApplyFilterAndReload()
        {
            //dgvEntries.Rows.Clear();
            _displayedEntries.Clear();
            if (_currentLog == null) return;

            var entries = _currentLog.Query(
                from: since == DateTime.MinValue ? (DateTime?)null : since,
                minSeverity: severity,
                source: string.IsNullOrEmpty(txtSource.Text) ? null : txtSource.Text,
                assembly: string.IsNullOrEmpty(txtAssembly.Text) ? null : txtAssembly.Text,
                className: string.IsNullOrEmpty(txtClass.Text) ? null : txtClass.Text,
                method: string.IsNullOrEmpty(txtMethod.Text) ? null : txtMethod.Text,
                keyword: string.IsNullOrEmpty(txtKeyword.Text) ? null : txtKeyword.Text
            );

            foreach (var entry in entries)
            {
                AddEntryToGrid(entry);
            }
        }
        private void AddEntryToGrid(RichLogEntry entry)
        {
            _displayedEntries.Add(entry);
            //dgvEntries.Rows.Add(
            //    entry.Timestamp,
            //    entry.Severity.ToString(),
            //    entry.Source,
            //    entry.AssemblyName,
            //    entry.ClassName,
            //    entry.MethodName,
            //    entry.Message
            //);
        }
        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (_currentLog == null)
                return;

            // Resubscribe with new filter
            if (_currentHandler != null)
                _currentLog.Unsubscribe(_currentHandler);

            _currentHandler = (s, entry) =>
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => AddEntryToGrid(entry)));
                }
                else
                {
                    AddEntryToGrid(entry);
                }
            };

            txtStatus.Text = "Filter Active";
            btnApplyFilter.Enabled = false;

            ApplyFilterAndReload();
            _currentLog.Subscribe(MakeFilterFunc(), _currentHandler);
        }
        private void txtSince_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSince.Text))
            {
                since = DateTime.MinValue;
            }
            else if (!DateTime.TryParse(txtSince.Text, out since))
            {
                since = DateTime.MinValue;
                MessageBox.Show("Invalid date/time format for 'Since'. Please use a valid date/time.");
                e.Cancel = true;
            }
        }
        private void cboSeverity_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeactivateFilter();
        }
        private void DeactivateFilter()
        {
            if (_currentHandler != null)
                _currentLog.Unsubscribe(_currentHandler);
            txtStatus.Text = "Not Active";
            btnApplyFilter.Enabled = true;
        }
        private void txt_Validated(object sender, EventArgs e)
        {
            if (sender is TextBoxPlaceHolder txtph)
            {
                if (txtph.Text != txtph.PreviousText)
                {
                    DeactivateFilter();
                }
            }
        }
        private byte severity = 0;
        private void txtSeverity_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSeverity.Text))
            {
                severity = 0;
            }
            else if (!byte.TryParse(txtSeverity.Text, out severity))
            {
                severity = 0;
                MessageBox.Show("Invalid number format for 'Minimum Severity'. Please use a valid byte value (0-255).");
                e.Cancel = true;
            }
        }

        private void dgvEntries_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var detailsForm = new frmLogDetailViewer(_displayedEntries, e.RowIndex);
                detailsForm.ShowInstance();
            }
        }
    }
}

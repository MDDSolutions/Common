using MDDFoundation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmLogDetailViewer : Form
    {
        private const int MarginLeft = 12;
        private const int LabelWidth = 90;
        private const int ColumnSpacing = 16;
        private const int RowHeight = 24;
        private const int VerticalSpacing = 4;

        public frmLogViewer ParentLogViewer { get; set; }
        public void GoTo(int newIndex)
        {
            if (newIndex >= 0 && newIndex < richLogEntryBindingSource.Count && richLogEntryBindingSource.Position != newIndex)
            {
                richLogEntryBindingSource.Position = newIndex;
            }
        }

        private Type _lastType;

        public frmLogDetailViewer()
        {
            InitializeComponent();

            this.Load += FrmLogDetailViewer_Load;
            this.Resize += FrmLogDetailViewer_Resize;
            this.richLogEntryBindingSource.CurrentChanged += RichLogEntryBindingSource_CurrentChanged;
        }

        public frmLogDetailViewer(BindingList<RichLogEntry> entries, int rowindex) : this()
        {
            richLogEntryBindingSource.DataSource = entries;
            if (rowindex >= 0 && rowindex < entries.Count)
                richLogEntryBindingSource.Position = rowindex;
        }

        private void FrmLogDetailViewer_Load(object sender, EventArgs e)
        {
            AdjustLayout();
            RebuildDynamicUI(); // initial build
        }

        private void FrmLogDetailViewer_Resize(object sender, EventArgs e)
        {
            AdjustLayout();
            // no forced rebuild - TableLayoutPanel will resize controls
        }

        private void RichLogEntryBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            var current = richLogEntryBindingSource.Current;
            var type = current.GetType();
            if (type == _lastType)
            {
                SetValues();
            }
            else
            {
                _lastType = type;
                RebuildDynamicUI();
            }
            ParentLogViewer?.GoTo(richLogEntryBindingSource.Position);
        }
        private void SetValues()
        {
            var current = richLogEntryBindingSource.Current;
            foreach (var chk in tlpFields.Controls.OfType<CheckBox>())
            {
                if (current == null)
                    chk.Checked = false;
                else if (chk.Tag is PropertyInfo prop)
                {
                    var val = prop.GetValue(current);
                    if (val is bool b)
                        chk.Checked = b;
                }
            }
            foreach (var txt in tlpFields.Controls.OfType<TextBox>())
            {
                if (current == null)
                    txt.Text = string.Empty;
                else if (txt.Tag is PropertyInfo prop)
                {
                    var val = prop.GetValue(current);
                    txt.Text = val?.ToString() ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Build the TL panel rows/controls for the current runtime type. Only rebuild when the type changes.
        /// </summary>
        private void RebuildDynamicUI()
        {
            var current = richLogEntryBindingSource.Current;
            if (current == null)
            {
                tlpFields.SuspendLayout();
                tlpFields.Controls.Clear();
                tlpFields.RowStyles.Clear();
                tlpFields.RowCount = 0;
                tlpFields.ResumeLayout();
                _lastType = null;
                AdjustLayout();
                return;
            }



            // Get properties in declaration order (metadata token heuristic)
            //var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //                .Where(p => p.CanRead && p.CanWrite && p.Name != nameof(RichLogEntry.Details))
            //                .OrderBy(p => p.MetadataToken) // preserves declared ordering in common cases
            //                .ToArray();
            var type = _lastType;
            var props = GetOrderedProperties(type);

            tlpFields.SuspendLayout();
            tlpFields.Controls.Clear();
            tlpFields.RowStyles.Clear();

            // ensure table configured for 4 columns (label/control left, label/control right)
            tlpFields.ColumnCount = 4;
            // ColumnStyles already set in designer; keep as-is.

            // Determine number of rows
            int rows = (props.Length + 1) / 2;
            tlpFields.RowCount = Math.Max(0, rows);

            for (int r = 0; r < rows; r++)
            {
                tlpFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                int colGroup = (i % 2 == 0) ? 0 : 1; // 0 => left, 1 => right
                int row = i / 2;
                int labelColumn = (colGroup == 0) ? 0 : 2;
                int controlColumn = (colGroup == 0) ? 1 : 3;

                // Label
                var lbl = new Label
                {
                    Text = prop.Name + ":",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(3, 3, 3, 3)
                };
                lbl.Width = LabelWidth;
                lbl.Anchor = AnchorStyles.Left | AnchorStyles.Top;

                // Control
                Control ctrl;
                if (prop.PropertyType == typeof(bool))
                {
                    var chk = new CheckBox
                    {
                        AutoSize = true,
                        Margin = new Padding(3, 3, 3, 3),
                        Anchor = AnchorStyles.Left | AnchorStyles.Top
                    };
                    // bind Checked
                    //chk.DataBindings.Add("Checked", richLogEntryBindingSource, prop.Name, true, DataSourceUpdateMode.OnPropertyChanged);
                    chk.Tag = prop;
                    chk.Checked = prop.GetValue(current) is bool b && b;
                    ctrl = chk;
                }
                else
                {
                    var txt = new TextBox
                    {
                        Margin = new Padding(3, 3, 3, 3),
                        Dock = DockStyle.Fill // expand horizontally
                    };
                    //txt.DataBindings.Add("Text", richLogEntryBindingSource, prop.Name, true, DataSourceUpdateMode.OnPropertyChanged);
                    txt.Tag = prop;
                    txt.Text = prop.GetValue(current)?.ToString() ?? string.Empty;
                    ctrl = txt;
                }

                // Ensure unique names (helpful for debug)
                lbl.Name = "lblDyn_" + prop.Name + "_" + i;
                ctrl.Name = "ctrlDyn_" + prop.Name + "_" + i;

                // Add label and control into the table
                // If rows were too few, ensure AddPosition is valid
                tlpFields.Controls.Add(lbl, labelColumn, row);
                tlpFields.Controls.Add(ctrl, controlColumn, row);
            }

            // Let the TableLayoutPanel calculate its preferred size for the current width
            // First ensure it knows the width it should layout into
            int clientWidth = Math.Max(100, this.ClientSize.Width - (MarginLeft * 2));
            tlpFields.Width = clientWidth;

            // Force layout to compute preferred size
            tlpFields.PerformLayout();
            var preferred = tlpFields.GetPreferredSize(new Size(clientWidth, 0));
            int requiredHeight = preferred.Height;

            int halfHeight = Math.Max(100, this.ClientSize.Height / 2);
            if (requiredHeight > halfHeight)
            {
                tlpFields.Height = halfHeight;
                tlpFields.AutoScroll = true;
            }
            else
            {
                tlpFields.Height = requiredHeight;
                tlpFields.AutoScroll = false;
            }

            tlpFields.ResumeLayout();
            AdjustLayout();
        }

        /// <summary>
        /// Reposition and size tlpFields and details area so everything lines up. Table columns determine the textbox span.
        /// </summary>
        private void AdjustLayout()
        {
            // Ensure tlpFields width spans client area with margin
            int clientWidth = Math.Max(100, this.ClientSize.Width - (MarginLeft * 2));
            tlpFields.Left = MarginLeft;
            tlpFields.Width = clientWidth;

            // Compute column widths (the TableLayoutPanel must have done layout already)
            int[] colWidths = tlpFields.GetColumnWidths(); // returns pixel widths for each column
            int col0Width = (colWidths.Length > 0) ? colWidths[0] : 0;

            // Label (Details) aligns with left column labels
            int gap = 8;
            lblDetails.Left = tlpFields.Left;
            lblDetails.Top = tlpFields.Top + tlpFields.Height + gap;

            // Text details left aligned with first-column textboxes (i.e. after column 0)
            int detailsLeft = tlpFields.Left + col0Width + 4;
            int availableRight = tlpFields.Left + tlpFields.Width - 4;
            int detailsWidth = Math.Max(100, availableRight - detailsLeft);

            // Position details textbox
            txtDetails.Left = detailsLeft;
            txtDetails.Top = lblDetails.Top - 4;
            txtDetails.Width = detailsWidth;

            // Height: fill to above navigator
            int navHeight = richLogEntryBindingNavigator.Height;
            int availableHeight = Math.Max(60, this.ClientSize.Height - txtDetails.Top - navHeight - 12);
            txtDetails.Height = availableHeight;
            txtDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
        private static PropertyInfo[] GetOrderedProperties(Type type)
        {
            var types = new System.Collections.Generic.List<Type>();
            while (type != null && type != typeof(object))
            {
                types.Insert(0, type); // insert at start to get base first
                type = type.BaseType;
            }

            var props = types
                .SelectMany(t => t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.Name != nameof(RichLogEntry.Details))
                    .OrderBy(p => p.MetadataToken))
                .ToArray();

            return props;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MDDWinForms
{
    /// <summary>
    /// Describes a single input field for frmInput.
    /// </summary>
    public class InputField
    {
        public string Label { get; }
        public string DefaultValue { get; }
        public IList<string> Suggestions { get; }
        public bool IsPassword { get; }

        public InputField(string label, string defaultValue = null, IEnumerable<string> suggestions = null, bool isPassword = false)
        {
            Label = label;
            DefaultValue = defaultValue;
            Suggestions = suggestions?.ToList() ?? new List<string>();
            IsPassword = isPassword;
        }
    }

    /// <summary>
    /// Generic input dialog. Supports single or multi-field forms, optional dropdown suggestions, and password masking.
    /// </summary>
    /// <example>
    /// // Simple (backward-compatible)
    /// using (var dlg = new frmInput("Rename", "New name:", currentName))
    ///     if (dlg.ShowDialog() == DialogResult.OK) name = dlg.Value;
    ///
    /// // With suggestions
    /// using (var dlg = new frmInput("Choose server", "Server:", new[] { "prod", "dev", "local" }))
    ///     if (dlg.ShowDialog() == DialogResult.OK) server = dlg.Value;
    ///
    /// // Multi-field
    /// var dlg = new frmInput("NAS Login", new[]
    /// {
    ///     new InputField("Username:", "superuser"),
    ///     new InputField("Password:", isPassword: true),
    /// });
    /// if (dlg.ShowDialog() == DialogResult.OK)
    ///     Login(dlg.Values["Username:"], dlg.Values["Password:"]);
    /// </example>
    public partial class frmInput : Form
    {
        private readonly List<InputField> _fields;
        private readonly List<Control> _inputControls = new List<Control>();

        // Backward-compatible: single text field
        public frmInput(string title, string label, string defaultValue = null)
        {
            _fields = new List<InputField> { new InputField(label, defaultValue) };
            InitializeComponent();
            Text = title;
            BuildSingleLayout(_fields[0]);
        }

        // Single field with suggested-values dropdown
        public frmInput(string title, string label, IEnumerable<string> suggestions, string defaultValue = null)
        {
            _fields = new List<InputField> { new InputField(label, defaultValue, suggestions) };
            InitializeComponent();
            Text = title;
            BuildSingleLayout(_fields[0]);
        }

        // Multi-field form
        public frmInput(string title, IEnumerable<InputField> fields)
        {
            _fields = fields.ToList();
            if (_fields.Count == 0) throw new ArgumentException("At least one field required.", nameof(fields));
            InitializeComponent();
            Text = title;
            if (_fields.Count == 1)
                BuildSingleLayout(_fields[0]);
            else
                BuildMultiLayout();
        }

        // First field value — backward-compatible
        public string Value => _inputControls.Count > 0 ? GetControlValue(_inputControls[0]) : null;

        // All field values keyed by label, populated after ShowDialog
        public IReadOnlyDictionary<string, string> Values
        {
            get
            {
                var d = new Dictionary<string, string>();
                for (int i = 0; i < _fields.Count && i < _inputControls.Count; i++)
                    d[_fields[i].Label] = GetControlValue(_inputControls[i]);
                return d;
            }
        }

        private static string GetControlValue(Control c)
            => c is ComboBox cb ? cb.Text : c.Text;

        private Control CreateInput(InputField field)
        {
            if (field.Suggestions.Count > 0)
            {
                var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
                foreach (var s in field.Suggestions)
                    combo.Items.Add(s);
                combo.Text = field.DefaultValue ?? (combo.Items.Count > 0 ? combo.Items[0].ToString() : "");
                return combo;
            }

            var txt = new TextBox();
            if (field.IsPassword) txt.UseSystemPasswordChar = true;
            if (field.DefaultValue != null) txt.Text = field.DefaultValue;
            return txt;
        }

        // Single-field layout: label stacked above input (handles any label length)
        private void BuildSingleLayout(InputField field)
        {
            const int pad = 12;
            const int inputH = 26;
            const int btnH = 30;
            const int btnW = 82;
            const int gap = 8;
            const int formW = 460;
            int innerW = formW - pad * 2;

            var labelSz = TextRenderer.MeasureText(field.Label, Font, new Size(innerW, 9999), TextFormatFlags.WordBreak);
            int labelH = labelSz.Height;

            ClientSize = new Size(formW, pad + labelH + gap + inputH + gap + btnH + pad);

            Controls.Add(new Label
            {
                Text = field.Label,
                Location = new Point(pad, pad),
                Size = new Size(innerW, labelH),
            });

            var input = CreateInput(field);
            input.Location = new Point(pad, pad + labelH + gap);
            input.Size = new Size(innerW, inputH);
            Controls.Add(input);
            _inputControls.Add(input);

            int buttonY = pad + labelH + gap + inputH + gap;
            AddButtons(formW, buttonY, btnW, btnH, pad, gap);
        }

        // Multi-field layout: label to the left of each input, auto-sized label column
        private void BuildMultiLayout()
        {
            const int pad = 12;
            const int inputH = 26;
            const int btnH = 30;
            const int btnW = 82;
            const int rowGap = 6;
            const int gap = 8;
            const int formW = 500;

            int maxLabelW = _fields.Select(f => TextRenderer.MeasureText(f.Label, Font).Width).Max();
            maxLabelW = Math.Max(80, Math.Min(maxLabelW + 8, 220));

            int inputX = pad + maxLabelW + gap;
            int inputW = formW - inputX - pad;
            int rowH = inputH + rowGap;
            int fieldsH = _fields.Count * rowH - rowGap;

            ClientSize = new Size(formW, pad + fieldsH + gap + btnH + pad);

            for (int i = 0; i < _fields.Count; i++)
            {
                int y = pad + i * rowH;
                var field = _fields[i];

                Controls.Add(new Label
                {
                    Text = field.Label,
                    Location = new Point(pad, y + 3), // +3 to visually center with textbox
                    Size = new Size(maxLabelW, inputH),
                    TextAlign = ContentAlignment.MiddleRight,
                });

                var input = CreateInput(field);
                input.Location = new Point(inputX, y);
                input.Size = new Size(inputW, inputH);
                Controls.Add(input);
                _inputControls.Add(input);
            }

            AddButtons(formW, pad + fieldsH + gap, btnW, btnH, pad, gap);
        }

        private void AddButtons(int formW, int y, int btnW, int btnH, int pad, int gap)
        {
            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(formW - pad - btnW * 2 - gap, y),
                Size = new Size(btnW, btnH),
            };
            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(formW - pad - btnW, y),
                Size = new Size(btnW, btnH),
            };
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }
    }
}

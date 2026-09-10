using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MDDFoundation.Menus;
using MenuItem = MDDFoundation.Menus.MenuItem;

namespace MDDWinForms.Menus
{
    /// <summary>Edits one menu item. The visible fields follow the target kind, because the database
    /// only accepts specific combinations - a free-form editor would produce constraint violations
    /// the user cannot act on.</summary>
    public class MenuItemEditDialog : Form
    {
        private readonly MenuItem item;
        private readonly IReadOnlyList<MenuItem> allItems;
        private readonly TableLayoutPanel layout = new TableLayoutPanel {
            Dock = DockStyle.Fill, ColumnCount = 2, AutoScroll = true, Padding = new Padding(12)
        };
        private readonly TextBox title = new TextBox();
        private readonly TextBox description = new TextBox();
        private readonly TextBox keywords = new TextBox();
        private readonly ComboBox targetKind = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox targetTypeName = new TextBox();
        private readonly TextBox assemblyName = new TextBox();
        private readonly TextBox procedureName = new TextBox();
        private readonly TextBox executablePath = new TextBox();
        private readonly TextBox arguments = new TextBox();
        private readonly ComboBox launchMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly CheckBox allowNewInstance = new CheckBox { Text = "Allow more than one instance", AutoSize = true };
        private readonly ComboBox parentCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly CheckedListBox categories = new CheckedListBox { CheckOnClick = true, IntegralHeight = false };
        private readonly Label problem = new Label { AutoSize = true, ForeColor = Color.Firebrick };
        private readonly Dictionary<Control, Label> captions = new Dictionary<Control, Label>();

        /// <summary>The edited item. Categories carry the chosen membership; order is set in the launcher.</summary>
        public MenuItem Result => item;

        public MenuItemEditDialog(MenuItem editing, IReadOnlyList<MenuItem> allItems)
        {
            item = editing ?? throw new ArgumentNullException(nameof(editing));
            this.allItems = allItems ?? throw new ArgumentNullException(nameof(allItems));
            var isCategory = item.Kind == MenuItemKind.Category;
            Text = (item.Id > 0 ? "Edit " : "New ") + (isCategory ? "category" : "menu item");
            // Sizable with an explicit size: a Form that auto-sizes around docked children has no
            // width to hand them, and they collapse.
            FormBorderStyle = FormBorderStyle.Sizable;
            SizeGripStyle = SizeGripStyle.Show;
            MinimizeBox = false; MaximizeBox = false; ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);
            ClientSize = isCategory ? new Size(560, 260) : new Size(600, 620);
            MinimumSize = new Size(420, isCategory ? 240 : 400);

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            AddRow("Title", title);
            AddRow("Description", description);
            AddRow("Keywords", keywords);
            if (isCategory)
            {
                AddRow("Parent category", parentCategory);
            }
            else
            {
                AddRow("Opens", targetKind);
                AddRow("Type name", targetTypeName);
                AddRow("Assembly", assemblyName);
                AddRow("Procedure", procedureName);
                AddRow("Program", executablePath);
                AddRow("Arguments", arguments);
                AddRow("Opening it", launchMode);
                AddRow("", allowNewInstance);
                AddRow("In categories", categories, grows: true);
            }
            problem.Margin = new Padding(3, 8, 3, 3);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(problem, 1, layout.RowCount++);

            var buttons = new FlowLayoutPanel {
                Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(12)
            };
            var ok = new Button { Text = "OK", AutoSize = true, DialogResult = DialogResult.None };
            var cancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
            buttons.Controls.Add(ok); buttons.Controls.Add(cancel);
            // The filling control is added first so the docked buttons claim their edge first.
            Controls.Add(layout); Controls.Add(buttons);
            AcceptButton = ok; CancelButton = cancel;

            targetKind.Items.AddRange(new object[] { "A form or control", "An external program", "A stored procedure" });
            launchMode.Items.AddRange(new object[] { "Reuses an open window", "Always opens a new window" });
            targetKind.SelectedIndexChanged += (s, e) => ApplyTargetKind();
            ok.Click += (s, e) => { if (Commit()) DialogResult = DialogResult.OK; };
            Shown += (s, e) => title.Focus();
            Populate();
        }

        /// <summary>One label/editor row. The editor stretches with the dialog; a growing row takes
        /// the leftover height.</summary>
        private void AddRow(string caption, Control editor, bool grows = false)
        {
            var row = layout.RowCount++;
            layout.RowStyles.Add(grows ? new RowStyle(SizeType.Percent, 100) : new RowStyle(SizeType.AutoSize));
            var label = new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 7, 8, 3) };
            if (grows) label.Anchor = AnchorStyles.Left;
            if (editor is CheckBox) editor.Anchor = AnchorStyles.Left;
            else if (grows) editor.Dock = DockStyle.Fill;
            else editor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            if (grows) editor.MinimumSize = new Size(0, 120);
            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(editor, 1, row);
            captions[editor] = label;
        }
        private void Show(Control editor, bool visible)
        {
            editor.Visible = visible;
            captions[editor].Visible = visible;
        }

        private void Populate()
        {
            title.Text = item.Title ?? "";
            description.Text = item.Description ?? "";
            keywords.Text = item.Keywords ?? "";
            if (item.Kind == MenuItemKind.Category)
            {
                parentCategory.Items.Add(new Option(null, "(top level)"));
                foreach (var category in Categories().Where(c => c.Id != item.Id && !IsDescendantOf(c, item.Id)))
                    parentCategory.Items.Add(new Option(category.Id, PathOf(category)));
                parentCategory.SelectedIndex = Math.Max(0, parentCategory.Items.Cast<Option>()
                    .ToList().FindIndex(o => o.Id == item.ParentId));
                return;
            }
            targetKind.SelectedIndex = (int)item.TargetKind;
            targetTypeName.Text = item.TargetTypeName ?? "";
            assemblyName.Text = item.AssemblyName ?? "";
            procedureName.Text = item.ProcedureName ?? "";
            executablePath.Text = item.ExecutablePath ?? "";
            arguments.Text = item.Arguments ?? "";
            launchMode.SelectedIndex = (int)item.DefaultLaunchMode;
            allowNewInstance.Checked = item.AllowNewInstance;
            foreach (var category in Categories())
                categories.Items.Add(new Option(category.Id, PathOf(category)),
                    item.Categories.Any(c => c.CategoryId == category.Id));
            ApplyTargetKind();
        }

        private void ApplyTargetKind()
        {
            var kind = (MenuTargetKind)Math.Max(0, targetKind.SelectedIndex);
            Show(targetTypeName, kind == MenuTargetKind.Control);
            Show(assemblyName, kind == MenuTargetKind.Control);
            Show(procedureName, kind == MenuTargetKind.StoredProcedure);
            Show(executablePath, kind == MenuTargetKind.Process);
            Show(arguments, kind == MenuTargetKind.Process);
            // An external program always gets its own window; the database enforces this too.
            if (kind == MenuTargetKind.Process) launchMode.SelectedIndex = (int)MenuLaunchMode.CreateNew;
            launchMode.Enabled = kind != MenuTargetKind.Process;
        }

        /// <summary>Writes the fields back onto the item, or reports why it cannot.</summary>
        private bool Commit()
        {
            problem.Text = "";
            item.Title = title.Text.Trim();
            item.Description = Blank(description.Text);
            item.Keywords = Blank(keywords.Text);
            if (item.Kind == MenuItemKind.Category)
            {
                item.ParentId = ((Option)parentCategory.SelectedItem)?.Id;
            }
            else
            {
                var kind = (MenuTargetKind)Math.Max(0, targetKind.SelectedIndex);
                item.TargetKind = kind;
                item.TargetTypeName = kind == MenuTargetKind.Control ? Blank(targetTypeName.Text) : null;
                item.AssemblyName = kind == MenuTargetKind.Control ? Blank(assemblyName.Text) : null;
                item.ProcedureName = kind == MenuTargetKind.StoredProcedure ? Blank(procedureName.Text) : null;
                item.ExecutablePath = kind == MenuTargetKind.Process ? Blank(executablePath.Text) : null;
                item.Arguments = kind == MenuTargetKind.Process ? Blank(arguments.Text) : null;
                item.DefaultLaunchMode = kind == MenuTargetKind.Process ? MenuLaunchMode.CreateNew : (MenuLaunchMode)Math.Max(0, launchMode.SelectedIndex);
                item.AllowNewInstance = allowNewInstance.Checked;
                var chosen = categories.CheckedItems.Cast<Option>().Select(o => o.Id.Value).ToList();
                if (chosen.Count == 0) return Fail("A menu item has to be in at least one category.");
                var existing = item.Categories.ToDictionary(c => c.CategoryId);
                item.Categories = chosen.Select(id => existing.TryGetValue(id, out var had)
                    ? had : new MenuCategoryRef { CategoryId = id, SortOrder = 0 }).ToList();
            }
            // Validate against the whole definition so the same rules that gate loading gate saving.
            // A new item has no id until the editor saves it, and Validate requires a positive one -
            // so stand in an unused id for the check and put it back afterwards.
            var actualId = item.Id;
            if (actualId <= 0) item.Id = allItems.Count == 0 ? 1 : allItems.Max(i => i.Id) + 1;
            try { MenuDefinition.Validate(allItems.Where(i => i.Id != item.Id).Concat(new[] { item }).ToList()); }
            catch (Exception ex) { return Fail(ex.Message); }
            finally { item.Id = actualId; }
            return true;
        }
        private bool Fail(string message) { problem.Text = message; return false; }
        private static string Blank(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private IEnumerable<MenuItem> Categories() =>
            allItems.Where(i => i.Kind == MenuItemKind.Category).OrderBy(PathOf, StringComparer.CurrentCultureIgnoreCase);
        private bool IsDescendantOf(MenuItem category, int ancestorId)
        {
            while (category?.ParentId != null)
            {
                if (category.ParentId.Value == ancestorId) return true;
                category = allItems.FirstOrDefault(i => i.Id == category.ParentId.Value);
            }
            return false;
        }
        private string PathOf(MenuItem category)
        {
            var names = new List<string>();
            for (var current = category; current != null;
                 current = current.ParentId.HasValue ? allItems.FirstOrDefault(i => i.Id == current.ParentId.Value) : null)
                names.Insert(0, current.Title);
            return string.Join(" / ", names);
        }
        private sealed class Option
        {
            public int? Id { get; }
            private readonly string caption;
            public Option(int? id, string caption) { Id = id; this.caption = caption; }
            public override string ToString() => caption;
        }
    }
}

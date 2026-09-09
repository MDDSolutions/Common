using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MDDFoundation.Menus;
using MenuItem = MDDFoundation.Menus.MenuItem;

namespace MDDWinForms.Menus
{
    /// <summary>Shared chrome for the small menu-editing prompts. These size themselves explicitly:
    /// a Form that auto-sizes around docked children has no width to give them, and they collapse.</summary>
    internal abstract class MenuPromptDialog : Form
    {
        protected readonly FlowLayoutPanel Body = new FlowLayoutPanel {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false,
            AutoScroll = true, Padding = new Padding(16)
        };

        protected MenuPromptDialog(string caption, string acceptText, Size size)
        {
            Text = caption;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false; MaximizeBox = false; ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);
            ClientSize = size;
            var buttons = new FlowLayoutPanel {
                Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(12)
            };
            var ok = new Button { Text = acceptText, AutoSize = true, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
            buttons.Controls.Add(ok); buttons.Controls.Add(cancel);
            // The filling control is added first so the docked buttons claim their edge first.
            Controls.Add(Body); Controls.Add(buttons);
            AcceptButton = ok; CancelButton = cancel;
        }
    }

    /// <summary>Asks whether Add means a category or a menu item.</summary>
    internal class AddChoiceDialog : MenuPromptDialog
    {
        private readonly RadioButton category = new RadioButton { Text = "A category", AutoSize = true, Checked = true };
        private readonly RadioButton action = new RadioButton { Text = "A menu item", AutoSize = true };
        public MenuItemKind Kind => category.Checked ? MenuItemKind.Category : MenuItemKind.Action;

        public AddChoiceDialog() : base("Add", "OK", new Size(320, 190))
        {
            Body.Controls.Add(new Label { Text = "What would you like to add?", AutoSize = true, Margin = new Padding(3, 3, 3, 10) });
            Body.Controls.Add(category);
            Body.Controls.Add(action);
        }
    }

    /// <summary>Asks where the contents of a category should go before it is deleted. A category is
    /// never emptied silently - its members would otherwise be left in no category at all.</summary>
    internal class CategoryChooserDialog : MenuPromptDialog
    {
        private readonly ComboBox destination = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        public int CategoryId => ((Option)destination.SelectedItem).Id;

        public CategoryChooserDialog(MenuItem deleting, int occupants, IReadOnlyList<MenuItem> destinations)
            : base("Delete " + deleting.Title, "Move and delete", new Size(380, 190))
        {
            Body.Controls.Add(new Label {
                Text = $"{deleting.Title} still holds {occupants} item{(occupants == 1 ? "" : "s")}." +
                       Environment.NewLine + "Move them to:",
                AutoSize = true, Margin = new Padding(3, 3, 3, 10)
            });
            Body.Controls.Add(destination);
            foreach (var option in destinations.OrderBy(c => c.Title, StringComparer.CurrentCultureIgnoreCase))
                destination.Items.Add(new Option(option.Id, option.Title));
            destination.SelectedIndex = 0;
        }
        private sealed class Option
        {
            public int Id { get; }
            private readonly string caption;
            public Option(int id, string caption) { Id = id; this.caption = caption; }
            public override string ToString() => caption;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public static class Prompt
    {
        public static string ShowDialog(string PromptText, string TitleBarCaption, string DefaultValue = null)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = TitleBarCaption,
                StartPosition = FormStartPosition.CenterParent
            };
            Label textLabel = new Label() { Left = 20, Top = 10, Text = PromptText };
            TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 480, Text = DefaultValue };
            Button confirmation = new Button() { Text = "Ok", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 380, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}

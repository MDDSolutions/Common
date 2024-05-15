using System;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class RadioButtonGroup : GroupBox
    {
        public event EventHandler SelectionChanged;
        public RadioButtonGroup()
        {
            this.ControlAdded += RadioButtonGroup_ControlAdded;
        }

        private void RadioButtonGroup_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is RadioButton radioButton)
            {
                radioButton.CheckedChanged += RadioButtonGroup_CheckedChanged;
            }
        }

        private void RadioButtonGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Checked)
            {
                SelectedValue = radioButton.Text;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string SelectedValue { get; private set; }

        public void SetSelectedValue(string value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is RadioButton radioButton)
                {
                    if (radioButton.Text == value)
                    {
                        if (!radioButton.Checked)
                            radioButton.Checked = true;
                        else
                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class FolderPickerButton : Button
    {
        public FolderPickerButton()
        {
            Text = "...";
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (LinkedControl != null)
            {
                var frm = new FolderPicker();
                if (!string.IsNullOrWhiteSpace(LinkedControl.Text))
                    frm.InputPath = LinkedControl.Text;
                if (frm.ShowDialog() == true)
                    LinkedControl.Text = frm.ResultPath;
            }
            else
            {
                MessageBox.Show("LinkedControl Cannot be null", "FolderPickerButton");
            }

        }
        public Control LinkedControl { get; set; }
    }
}

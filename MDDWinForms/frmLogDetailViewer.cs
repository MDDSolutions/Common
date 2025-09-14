using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class frmLogDetailViewer : Form
    {
        public frmLogDetailViewer()
        {
            InitializeComponent();
        }
        public frmLogDetailViewer(BindingList<RichLogEntry> entries, int rowindex) : this()
        {
            richLogEntryBindingSource.DataSource = entries;
            if (rowindex >= 0 && rowindex < entries.Count)
                richLogEntryBindingSource.Position = rowindex;
        }
    }
}

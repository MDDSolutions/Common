using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class ToolStripMenuItemWithContext<T> : ToolStripMenuItem
    {
        public ToolStripMenuItemWithContext() : base() {}
        public ToolStripMenuItemWithContext(string text, EventHandler onclick) : base(text, null, onclick) {}
        public ToolStripMenuItemWithContext(string text, EventHandler onclick, T context) : base(text, null, onclick) => ContextObject = context;
        public ToolStripMenuItemWithContext(string text, T context, Action<T> action) : base(text)
        {
            ClickAction = action;
            ContextObject = context;
        }
        public T ContextObject { get; set; }
        public Action<T> ClickAction { get; set; } = null;
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ClickAction?.Invoke(ContextObject);
        }
    }
}

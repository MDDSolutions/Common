using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public class FormDirtyTracker
    {
        public event EventHandler<bool> DirtyChanged;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value != _isDirty)
                {
                    _isDirty = value;
                    DirtyChanged?.Invoke(this, value);
                }
            }
        }
        public FormDirtyTracker(Form frm)
        {
            frmTracked = frm;
            controls = new List<ControlDirtyTracker>();
            AssignHandlersForControlCollection(frm.Controls);
        }

        public void ReInitialize()
        {
            controls = new List<ControlDirtyTracker>();
            AssignHandlersForControlCollection(frmTracked.Controls);
            IsDirty = controls.Where(x => x.DetermineIfDirty()).Any();
        }
        private Form frmTracked;
        private List<ControlDirtyTracker> controls;
        private bool _isDirty = false;

        private void UpdateIsDirtyEventHandler(object sender, EventArgs e)
        {
            IsDirty = controls.Where(x => x.DetermineIfDirty()).Any();
        }
        private void FormDirtyTracker_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if ((sender as DataGridView).IsCurrentCellDirty)
                IsDirty = true;
            else
                IsDirty = controls.Where(x => x.DetermineIfDirty()).Any();
        }
        private void AssignHandlersForControlCollection(Control.ControlCollection coll)
        {
            foreach (Control c in coll)
            {
                //Console.WriteLine(string.Format("{0}: {1}", c.Name, c.GetType().Name));
                if (c is TextBox)
                {
                    (c as TextBox).TextChanged += UpdateIsDirtyEventHandler;
                    controls.Add(new ControlDirtyTracker(c));
                }
                if (c is CheckBox)
                {
                    (c as CheckBox).CheckedChanged += UpdateIsDirtyEventHandler;
                    controls.Add(new ControlDirtyTracker(c));
                }
                if (c is DataGridView)
                {
                    (c as DataGridView).CurrentCellDirtyStateChanged += FormDirtyTracker_CurrentCellDirtyStateChanged;
                    (c as DataGridView).DataBindingComplete += UpdateIsDirtyEventHandler;
                    (c as DataGridView).UserAddedRow += UpdateIsDirtyEventHandler;
                    controls.Add(new ControlDirtyTracker(c));
                }
                if (c.HasChildren)
                    AssignHandlersForControlCollection(c.Controls);
            }
        }
    }
    public class ControlDirtyTracker
    {
        public override string ToString()
        {
            return String.Format("{0}: {1}", ctrlTracked.GetType().Name, CleanObject.ToString());
        }
        public Control ctrlTracked { get; set; }
        public Object CleanObject { get; set; }

        public ControlDirtyTracker(Control ctl)
        {
            if (ControlDirtyTracker.IsControlTypeSupported(ctl))
            {
                ctrlTracked = ctl;
                CleanObject = GetControlCurrentValue();
            }
            else
                throw new NotSupportedException(
                      string.Format(
                       "The control type for '{0}' "
                         + "is not supported by the ControlDirtyTracker class."
                        , ctl.Name)
                      );
        }
        public bool DetermineIfDirty()
        {
            if (CleanObject is String)
                return (string.Compare((CleanObject as string), (GetControlCurrentValue() as string), false) != 0);
            else if (CleanObject is List<List<Object>>)
            {
                var clean = (List<List<Object>>)CleanObject;
                var current = (List<List<Object>>)GetControlCurrentValue();

                if (clean.Count != current.Count) return true;

                for (int r = 0; r < clean.Count; r++)
                {
                    for (int c = 0; c < clean[r].Count; c++)
                    {
                        if (clean[r][c] == null)
                        { if (current[r][c] != null) return true; }
                        else
                            if (!clean[r][c].Equals(current[r][c])) return true;
                    }
                }
            }
            return false;
        }

        public static bool IsControlTypeSupported(Control ctl)
        {
            if (ctl is TextBox) return true;
            if (ctl is CheckBox) return true;
            if (ctl is ComboBox) return true;
            if (ctl is ListBox) return true;
            if (ctl is DataGridView) return true;
            return false;
        }
        private object GetControlCurrentValue()
        {
            if (ctrlTracked is TextBox)
                return (ctrlTracked as TextBox).Text;

            if (ctrlTracked is CheckBox)
                return (ctrlTracked as CheckBox).Checked.ToString();

            if (ctrlTracked is ComboBox)
                return (ctrlTracked as ComboBox).Text;

            if (ctrlTracked is ListBox)
            {
                StringBuilder val = new StringBuilder();
                ListBox lb = (ctrlTracked as ListBox);
                ListBox.SelectedIndexCollection coll = lb.SelectedIndices;
                for (int i = 0; i < coll.Count; i++)
                    val.AppendFormat("{0};", coll[i]);
                return val.ToString();
            }

            if (ctrlTracked is DataGridView)
            {
                var l = new List<List<Object>>();
                foreach (DataGridViewRow rw in (ctrlTracked as DataGridView).Rows)
                {
                    if (!rw.IsNewRow)
                    {
                        var il = new List<Object>();
                        foreach (DataGridViewColumn col in (ctrlTracked as DataGridView).Columns)
                        {
                            il.Add(rw.Cells[col.Index].Value);
                        }
                        l.Add(il);
                    }
                }
                return l;
            }

            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace FormsDataAccess
{
    // Two things to implement this component:
    // 1) Set DisplayMember to name of a property in object
    // 2) call SetDataSource with a list of objects 
    //      - can include DisplayMember here
    //      - possibly any enumerable will work, but not tested
    //
    // Optionally:
    // 1) Handle UserSelectionChanged event
    //      - will only fire when the Selection is actually different
    //      - will only fire when the Selection was not changed programmatically
    // 2) Handle the UserSearching event (will override default, inefficient rudimentary search):
    //      - use the text parameter to filter your list of objects
    //      - Set DataSource with filtered list

    [DefaultEvent("ValueChanged")]
    public partial class ctlDropDownSearch : ctlDropDownControl
    {
        public ctlDropDownSearch()
        {
            InitializeComponent();
            InitializeDropDown(pnlControls);
        }
        private IEnumerable originaldata = null;
        private PropertyInfo displayproperty = null;
        private PropertyInfo valueproperty = null;
        private Type type = null;
        private bool NullValue = true;
        // The binding source tracks the highlighted search result; this field tracks
        // the value the user actually committed with the mouse, Enter, or Tab.
        private object currentSelection = null;
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs eventargs)
        {
            ValueChanged?.Invoke(this, eventargs);
        }
        public bool TreatTextAsValue { get; set; } = false;
        public bool TreatZeroAsNull { get; set; } = true;
        public object DataSource
        {
            get 
            {
                if (DesignMode) return null;
                return bsListItems.DataSource; 
            }
        }
        public void SetDataSource(IEnumerable value, string displaymember = null, string valuemember = null)
        {
            if (originaldata == null)
            {
                foreach (var item in value)
                {
                    type = item.GetType();
                    break;
                }

                if (type == null)
                    throw new Exception("DataSource must not be empty in order to ascertain type of object");
                if (displaymember != null && DisplayMember != displaymember)
                    DisplayMember = displaymember;
                if (DisplayMember == null)
                    throw new Exception("DisplayMember must be set first or specified");
                if (valuemember != null && ValueMember != valuemember)
                    ValueMember = valuemember;
                if (ValueMember == null)
                    throw new Exception("ValueMember must be set first or specified");
                displayproperty = type.GetProperty(DisplayMember);
                valueproperty = type.GetProperty(ValueMember);

                originaldata = value;
            }
            lbxListItems.DisplayMember = DisplayMember;
            bsListItems.DataSource = value;
        }
        public string DisplayMember { get; set; } = "Must be set to valid property name";
        public string ValueMember { get; set; } = "Must be set to valid property name";
        public event EventHandler UserSelectionChanged;
        private void lbxListItems_MouseClick(object sender, MouseEventArgs e)
        {
            int index = lbxListItems.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches) return;

            lbxListItems.SelectedIndex = index;
            ConfirmCurrentItem();
        }
        public object CurrentSelection 
        { 
            get 
            {
                if (DesignMode) return null;
                if (NullValue) return null;
                return currentSelection;
            }
            set
            {
                bool selectionChanged = NullValue != (value == null) ||
                    (value != null && !value.Equals(currentSelection));

                if (value == null)
                {
                    NullValue = true;
                    currentSelection = null;
                    Text = null;
                    if (selectionChanged)
                        OnValueChanged(EventArgs.Empty);
                    return;
                }

                NullValue = false;
                currentSelection = value;
                int index = bsListItems.IndexOf(value);
                if (index >= 0)
                    bsListItems.Position = index;
                SetDisplayText(value);
                if (selectionChanged)
                    OnValueChanged(EventArgs.Empty);
            }
        }
        public object Value
        {
            get
            {
                if (DesignMode) return null;
                if (valueproperty == null) return null;
                if (NullValue) return null;
                return valueproperty.GetValue(CurrentSelection);
            }
            set
            {
                if (value == null)
                {
                    CurrentSelection = null;
                    return;
                }
                if (TreatZeroAsNull && long.TryParse(value.ToString(), out long lv))
                {
                    if (lv == 0)
                    {
                        CurrentSelection = null;
                        return;
                    }
                }
                bool found = false;
                foreach (var item in originaldata)
                {
                    if (valueproperty.GetValue(item).Equals(value))
                    {
                        found = true;
                        CurrentSelection = item;
                        break;
                    }
                }
                if (!found && !DesignMode) throw new KeyNotFoundException($"Key {value} not found in originaldata");
            }
        }
        public event EventHandler<string> UserSearching;
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (UserSearching != null)
            {
                UserSearching(this, txtSearch.Text);
            }
            else
            {
                if (originaldata != null)
                {
                    var l = new List<Object>();
                    foreach (var item in originaldata)
                    {
                        if (displayproperty.GetValue(item).ToString().IndexOf(txtSearch.Text, StringComparison.CurrentCultureIgnoreCase) != -1)
                            l.Add(item);
                    }
                    SetDataSource(l);
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
        }
        private void ctlDropDownSearch_Dropping(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }
        internal override bool ProcessDropDownKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            Keys modifiers = keyData & Keys.Modifiers;
            if (modifiers != Keys.None && !(keyCode == Keys.Tab && modifiers == Keys.Shift))
                return false;

            switch (keyCode)
            {
                case Keys.Tab:
                    ConfirmCurrentItem();
                    MoveFocusAfterDropDown((keyData & Keys.Shift) != Keys.Shift);
                    return true;
                case Keys.Enter:
                    ConfirmCurrentItem();
                    Focus();
                    return true;
                case Keys.Escape:
                    CloseDropDown();
                    Focus();
                    return true;
                case Keys.Down:
                    MoveCurrentItem(1);
                    return true;
                case Keys.Up:
                    MoveCurrentItem(-1);
                    return true;
                case Keys.PageDown:
                    MoveCurrentItem(Math.Max(1, lbxListItems.ClientSize.Height / lbxListItems.ItemHeight - 1));
                    return true;
                case Keys.PageUp:
                    MoveCurrentItem(-Math.Max(1, lbxListItems.ClientSize.Height / lbxListItems.ItemHeight - 1));
                    return true;
                case Keys.Home:
                    MoveToItem(0);
                    return true;
                case Keys.End:
                    MoveToItem(bsListItems.Count - 1);
                    return true;
            }

            return false;
        }
        protected virtual void MoveFocusAfterDropDown(bool forward)
        {
            Control parent = Parent;
            if (parent != null)
                parent.SelectNextControl(this, forward, true, true, true);
        }
        private void MoveCurrentItem(int offset)
        {
            if (bsListItems.Count == 0) return;

            int position = bsListItems.Position;
            if (position < 0)
                position = offset > 0 ? -1 : bsListItems.Count;
            MoveToItem(position + offset);
        }
        private void MoveToItem(int position)
        {
            if (bsListItems.Count == 0) return;

            bsListItems.Position = Math.Max(0, Math.Min(position, bsListItems.Count - 1));
        }
        private void ConfirmCurrentItem()
        {
            if (bsListItems.Current != null)
                ConfirmSelection(bsListItems.Current);
            else
                CloseDropDown();
        }
        private void SetDisplayText(object selected)
        {
            if (selected == null)
            {
                Text = null;
                return;
            }

            if (!TreatTextAsValue)
                Text = displayproperty.GetValue(selected).ToString();
            else
                Text = valueproperty.GetValue(selected).ToString();
        }
        private void ConfirmSelection(object selected)
        {
            bool selectionChanged = NullValue || !selected.Equals(currentSelection);
            NullValue = false;
            currentSelection = selected;
            SetDisplayText(selected);
            CloseDropDown();
            if (selectionChanged)
            {
                OnValueChanged(EventArgs.Empty);
                UserSelectionChanged?.Invoke(this, null);
            }
        }
    }
}

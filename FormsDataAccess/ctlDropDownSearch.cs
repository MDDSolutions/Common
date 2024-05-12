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
            searching = true;
            lbxListItems.DisplayMember = DisplayMember;
            bsListItems.DataSource = value;
            searching = false;
        }
        public string DisplayMember { get; set; } = "Must be set to valid property name";
        public string ValueMember { get; set; } = "Must be set to valid property name";
        public event EventHandler UserSelectionChanged;
        private object lastselected = null;
        private bool searching = false;
        private void lbxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DropState == eDropState.Dropping || this.DropState == eDropState.Closing) return;
            if (searching) return;
            if (lbxListItems.SelectedItems.Count == 0) return;
            ConfirmSelection(lbxListItems.SelectedItems[0]);
        }
        public object CurrentSelection 
        { 
            get 
            {
                if (DesignMode) return null;
                if (NullValue) return null;
                return bsListItems.Current; 
            }
            set
            {
                if (value == null)
                {
                    NullValue = true;
                    Text = null;
                    return;
                }
                else
                {
                    NullValue = false;
                }
                int index = bsListItems.IndexOf(value);


                //causing errors - not sure why...
                //if (index == -1 && !DesignMode)
                //    throw new KeyNotFoundException($"Item {value} not found in bsListItems");



                //lastselected = value; //this prevents UserSelectionChanged event from firing when Selection is changed programmatically
                //except that it is also preventing the event from firing when it is changed by the User - which defeats the purpose...
                bsListItems.Position = index;
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
        private void bsListItems_CurrentItemChanged(object sender, EventArgs e)
        {
            if (this.DropState == eDropState.Dropping || this.DropState == eDropState.Closing) return;
            if (bsListItems.Current == null) return;
            if (searching) return;

            var selected = bsListItems.Current;
            if (selected != null) ConfirmSelection(selected);
        }
        private void ConfirmSelection(object selected)
        {
            if (!TreatTextAsValue)
                Text = displayproperty.GetValue(selected).ToString();
            else
                Text = valueproperty.GetValue(selected).ToString();
            CurrentSelection = selected;
            CloseDropDown();
            OnValueChanged(null);
            if (lastselected == null)
            {
                if (selected != null)
                {
                    lastselected = selected;
                    UserSelectionChanged?.Invoke(this, null);
                }
            }
            else if (!lastselected.Equals(selected))
            {
                lastselected = selected;
                UserSelectionChanged?.Invoke(this, null);
            }
        }
    }
}

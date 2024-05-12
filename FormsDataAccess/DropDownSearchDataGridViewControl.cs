using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public class DropDownSearchColumn : DataGridViewColumn
    {
        public DropDownSearchColumn() : base(new DropDownSearchCell())
        {
        }
        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }

        private IEnumerable _datasource;
        public IEnumerable DataSource 
        { 
            get { return _datasource; }
            set
            {
                _datasource = value;
                if (value != null)
                {
                    KeyValues = new Dictionary<object, string>();
                    System.Reflection.PropertyInfo valprop = null;
                    System.Reflection.PropertyInfo dspprop = null;

                    foreach (var item in _datasource)
                    {
                        if (valprop == null)
                        {
                            valprop = item.GetType().GetProperty(ValueMember);
                            dspprop = item.GetType().GetProperty(DisplayMember);
                            if (valprop == null || dspprop == null)
                                throw new Exception("Either ValueMember or DisplayMember not found");
                        }
                        KeyValues.Add(valprop.GetValue(item), dspprop.GetValue(item).ToString());
                    }
                }
            } 
        }
        public override DataGridViewCell CellTemplate 
        {
            get { return base.CellTemplate; }
            set 
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DropDownSearchCell)))
                {
                    throw new InvalidCastException("Must be a DropDownSearchCell");
                }
                base.CellTemplate = value; 
            }
        }
        private Dictionary<object, string> KeyValues;
        public string GetDisplayValue(object key)
        {
            if (KeyValues != null && KeyValues.TryGetValue(key, out string value))
                return value;
            return null;
        }
    }
    public class DropDownSearchCell : DataGridViewTextBoxCell // DataGridViewComboBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var oc = OwningColumn as DropDownSearchColumn;
            var ctl = DataGridView.EditingControl as DropDownSearchEditingControl;
            if (ctl.DataSource == null)
            {
                ctl.TreatTextAsValue = true;
                ctl.SetDataSource(oc.DataSource, oc.DisplayMember, oc.ValueMember);
            }
            if (Value == null)
            {
                ctl.CurrentSelection = null;
            }
            else
            {
                ctl.Value = Value;
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        public override Type EditType => typeof(DropDownSearchEditingControl);
        public override Type ValueType => typeof(object);
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value != null)
            {
                var oc = OwningColumn as DropDownSearchColumn;
                return oc.GetDisplayValue(value);
                //if (long.TryParse(value.ToString(), out long lv))
                //{
                //    if (lv != 0)
                //    {
                //        //Maybe build a dictionary or something instead of doing this every time...
                //        var oc = OwningColumn as DropDownSearchColumn;
                //        System.Reflection.PropertyInfo prop = null;
                //        foreach (var item in oc.DataSource)
                //        {
                //            if (prop == null) prop = item.GetType().GetProperty(oc.ValueMember);
                //            if (prop.GetValue(item).Equals(value))
                //            {
                //                prop = item.GetType().GetProperty(oc.DisplayMember);
                //                return prop.GetValue(item);
                //            }
                //        }

                //    }
                //}
            }
            return null;
            //return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }
        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            //var ctl = this.DataGridView.EditingControl as DropDownSearchEditingControl;

            return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }
    }
    public class DropDownSearchEditingControl : ctlDropDownSearch, IDataGridViewEditingControl
    {
        public DataGridView EditingControlDataGridView { get; set; }
        public object EditingControlFormattedValue { get => base.Text; set => CurrentSelection = value; }
        public int EditingControlRowIndex { get; set; }
        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor { get => base.Cursor; }

        public bool RepositionEditingControlOnValueChange { get => false; }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.Font = dataGridViewCellStyle.Font;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }
        protected override void OnValueChanged(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }

}

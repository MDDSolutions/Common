using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    // This implementation of the DataGridView uses the BeginCellEdit event to save a copy of the data in a row (in a List<Object>) and then uses the RowValidating event
    // to compare current values to the saved copy.  If differences are found, it fires a custom ChangedRowValidating event allowing the user to cancel and then finally
    // a custom ChangedRowValidated event to indicate that the row has changed as a result of a user edit
    public class DataGridViewRowUpdate : DataGridView
    {
        public event EventHandler<DataGridViewRowEventArgs> UserChangedRowValidated;
        public event EventHandler<DataGridViewCellCancelEventArgs> UserChangedRowValidating;
        public event EventHandler RefreshRequested;

        private TextBox filtertextbox = null;
        public TextBox FilterTextBox
        {
            get { return filtertextbox; }
            set 
            { 
                filtertextbox = value;
                if (filtertextbox != null)
                    filtertextbox.TextChanged += Filtertextbox_TextChanged;
            }
        }
        DateTime lastupdate = DateTime.MaxValue;
        private async void Filtertextbox_TextChanged(object sender, EventArgs e)
        {
            lastupdate = DateTime.Now;
            await Task.Delay(300);
            var now = DateTime.Now;
            if (lastupdate.AddMilliseconds(290) <= now)
            {
                if (DataSource is BindingSource)
                {
                    if ((DataSource as BindingSource).DataSource is IBindingListView)
                    {
                        ((DataSource as BindingSource).DataSource as IBindingListView).Filter = filtertextbox.Text;
                        return;
                    }
                }
                if (DataSource is System.Data.DataTable)
                {
                    var dv = ((System.Data.DataTable)DataSource).DefaultView;
                    var filter = string.Empty;
                    foreach (System.Data.DataColumn col in dv.Table.Columns)
                    {
                        if (col.DataType == typeof(string))
                        {
                            if (filter != string.Empty) filter += " or ";
                            filter += $"[{col.ColumnName}] like '*{filtertextbox.Text}*'";
                        }
                        else
                        {
                            if (col.DataType == typeof(int) && int.TryParse(filtertextbox.Text, out int value))
                            {
                                if (filter != string.Empty) filter += " or ";
                                filter += $"[{col.ColumnName}] = {filtertextbox.Text}";
                            }
                        }
                    }
                    if (filter != string.Empty)
                        dv.RowFilter = filter; // "ProviderName like '*" + filtertextbox.Text + "*'";
                    return;
                }


                throw new NotImplementedException("filter method not implemented");

                //if (FilterMethod == null)
                //    throw new NotImplementedException("SearchMethod not Implemented");
                //if (DataSource is BindingSource)
                //    (DataSource as BindingSource).DataSource = FilterMethod(FilterTextBox.Text);
                //else
                //    DataSource = FilterMethod(FilterTextBox.Text);
            }
        }

        public DataGridViewRowUpdate()
        {
            //Console.WriteLine("//All Events:");
            //foreach (var item in this.GetType().GetEvents())
            //{
            //    Console.WriteLine($@"{item.Name} += (s,e) => Console.WriteLine(""{ item.Name}"");");
            //}
            //Console.WriteLine("// End of All Events:");
            //UserChangedRowValidated += (s, e) => PrintEvent("UserChangedRowValidated");
            //UserChangedRowValidating += (s, e) => PrintEvent("UserChangedRowValidating");
            //AllowUserToAddRowsChanged += (s, e) => PrintEvent("AllowUserToAddRowsChanged");
            //AllowUserToDeleteRowsChanged += (s, e) => PrintEvent("AllowUserToDeleteRowsChanged");
            //AllowUserToOrderColumnsChanged += (s, e) => PrintEvent("AllowUserToOrderColumnsChanged");
            //AllowUserToResizeColumnsChanged += (s, e) => PrintEvent("AllowUserToResizeColumnsChanged");
            //AllowUserToResizeRowsChanged += (s, e) => PrintEvent("AllowUserToResizeRowsChanged");
            //AlternatingRowsDefaultCellStyleChanged += (s, e) => PrintEvent("AlternatingRowsDefaultCellStyleChanged");
            //AutoGenerateColumnsChanged += (s, e) => PrintEvent("AutoGenerateColumnsChanged");
            //AutoSizeColumnsModeChanged += (s, e) => PrintEvent("AutoSizeColumnsModeChanged");
            //AutoSizeRowsModeChanged += (s, e) => PrintEvent("AutoSizeRowsModeChanged");
            //BackColorChanged += (s, e) => PrintEvent("BackColorChanged");
            //BackgroundColorChanged += (s, e) => PrintEvent("BackgroundColorChanged");
            //BackgroundImageChanged += (s, e) => PrintEvent("BackgroundImageChanged");
            //BackgroundImageLayoutChanged += (s, e) => PrintEvent("BackgroundImageLayoutChanged");
            //BorderStyleChanged += (s, e) => PrintEvent("BorderStyleChanged");
            //CellBorderStyleChanged += (s, e) => PrintEvent("CellBorderStyleChanged");
            //ColumnHeadersBorderStyleChanged += (s, e) => PrintEvent("ColumnHeadersBorderStyleChanged");
            //ColumnHeadersDefaultCellStyleChanged += (s, e) => PrintEvent("ColumnHeadersDefaultCellStyleChanged");
            //ColumnHeadersHeightChanged += (s, e) => PrintEvent("ColumnHeadersHeightChanged");
            //ColumnHeadersHeightSizeModeChanged += (s, e) => PrintEvent("ColumnHeadersHeightSizeModeChanged");
            //DataMemberChanged += (s, e) => PrintEvent("DataMemberChanged");
            //DataSourceChanged += (s, e) => PrintEvent("DataSourceChanged");
            //DefaultCellStyleChanged += (s, e) => PrintEvent("DefaultCellStyleChanged");
            //EditModeChanged += (s, e) => PrintEvent("EditModeChanged");
            //ForeColorChanged += (s, e) => PrintEvent("ForeColorChanged");
            //FontChanged += (s, e) => PrintEvent("FontChanged");
            //GridColorChanged += (s, e) => PrintEvent("GridColorChanged");
            //MultiSelectChanged += (s, e) => PrintEvent("MultiSelectChanged");
            //PaddingChanged += (s, e) => PrintEvent("PaddingChanged");
            //ReadOnlyChanged += (s, e) => PrintEvent("ReadOnlyChanged");
            //RowHeadersBorderStyleChanged += (s, e) => PrintEvent("RowHeadersBorderStyleChanged");
            //RowHeadersDefaultCellStyleChanged += (s, e) => PrintEvent("RowHeadersDefaultCellStyleChanged");
            //RowHeadersWidthChanged += (s, e) => PrintEvent("RowHeadersWidthChanged");
            //RowHeadersWidthSizeModeChanged += (s, e) => PrintEvent("RowHeadersWidthSizeModeChanged");
            //RowsDefaultCellStyleChanged += (s, e) => PrintEvent("RowsDefaultCellStyleChanged");
            //TextChanged += (s, e) => PrintEvent("TextChanged");
            //AutoSizeColumnModeChanged += (s, e) => PrintEvent("AutoSizeColumnModeChanged");
            //CancelRowEdit += (s, e) => PrintEvent("CancelRowEdit");
            //CellBeginEdit += (s, e) => PrintEvent("CellBeginEdit");
            //CellClick += (s, e) => PrintEvent("CellClick");
            //CellContentClick += (s, e) => PrintEvent("CellContentClick");
            //CellContentDoubleClick += (s, e) => PrintEvent("CellContentDoubleClick");
            //CellContextMenuStripChanged += (s, e) => PrintEvent("CellContextMenuStripChanged");
            //CellContextMenuStripNeeded += (s, e) => PrintEvent("CellContextMenuStripNeeded");
            //CellDoubleClick += (s, e) => PrintEvent("CellDoubleClick");
            //CellEndEdit += (s, e) => PrintEvent("CellEndEdit");
            ////CellEnter += (s, e) => PrintEvent("CellEnter");
            //CellErrorTextChanged += (s, e) => PrintEvent("CellErrorTextChanged");
            ////CellErrorTextNeeded += (s, e) => PrintEvent("CellErrorTextNeeded");
            ////CellFormatting += (s, e) => PrintEvent("CellFormatting");
            //CellLeave += (s, e) => PrintEvent("CellLeave");
            //CellMouseClick += (s, e) => PrintEvent("CellMouseClick");
            //CellMouseDoubleClick += (s, e) => PrintEvent("CellMouseDoubleClick");
            //CellMouseDown += (s, e) => PrintEvent("CellMouseDown");
            ////CellMouseEnter += (s, e) => PrintEvent("CellMouseEnter");
            ////CellMouseLeave += (s, e) => PrintEvent("CellMouseLeave");
            ////CellMouseMove += (s, e) => PrintEvent("CellMouseMove");
            //CellMouseUp += (s, e) => PrintEvent("CellMouseUp");
            //CellPainting += (s, e) => PrintEvent("CellPainting");
            //CellParsing += (s, e) => PrintEvent("CellParsing");
            //CellStateChanged += (s, e) => PrintEvent("CellStateChanged");
            //CellStyleChanged += (s, e) => PrintEvent("CellStyleChanged");
            //CellStyleContentChanged += (s, e) => PrintEvent("CellStyleContentChanged");
            //CellToolTipTextChanged += (s, e) => PrintEvent("CellToolTipTextChanged");
            ////CellToolTipTextNeeded += (s, e) => PrintEvent("CellToolTipTextNeeded");
            //CellValidated += (s, e) => PrintEvent("CellValidated");
            //CellValidating += (s, e) => PrintEvent("CellValidating");
            //CellValueChanged += (s, e) => PrintEvent("CellValueChanged");
            //CellValueNeeded += (s, e) => PrintEvent("CellValueNeeded");
            //CellValuePushed += (s, e) => PrintEvent("CellValuePushed");
            //ColumnAdded += (s, e) => PrintEvent("ColumnAdded");
            //ColumnContextMenuStripChanged += (s, e) => PrintEvent("ColumnContextMenuStripChanged");
            //ColumnDataPropertyNameChanged += (s, e) => PrintEvent("ColumnDataPropertyNameChanged");
            //ColumnDefaultCellStyleChanged += (s, e) => PrintEvent("ColumnDefaultCellStyleChanged");
            //ColumnDisplayIndexChanged += (s, e) => PrintEvent("ColumnDisplayIndexChanged");
            //ColumnDividerDoubleClick += (s, e) => PrintEvent("ColumnDividerDoubleClick");
            //ColumnDividerWidthChanged += (s, e) => PrintEvent("ColumnDividerWidthChanged");
            //ColumnHeaderMouseClick += (s, e) => PrintEvent("ColumnHeaderMouseClick");
            //ColumnHeaderMouseDoubleClick += (s, e) => PrintEvent("ColumnHeaderMouseDoubleClick");
            //ColumnHeaderCellChanged += (s, e) => PrintEvent("ColumnHeaderCellChanged");
            //ColumnMinimumWidthChanged += (s, e) => PrintEvent("ColumnMinimumWidthChanged");
            //ColumnNameChanged += (s, e) => PrintEvent("ColumnNameChanged");
            //ColumnRemoved += (s, e) => PrintEvent("ColumnRemoved");
            //ColumnSortModeChanged += (s, e) => PrintEvent("ColumnSortModeChanged");
            //ColumnStateChanged += (s, e) => PrintEvent("ColumnStateChanged");
            //ColumnToolTipTextChanged += (s, e) => PrintEvent("ColumnToolTipTextChanged");
            //ColumnWidthChanged += (s, e) => PrintEvent("ColumnWidthChanged");
            //CurrentCellChanged += (s, e) => PrintEvent("CurrentCellChanged");
            //CurrentCellDirtyStateChanged += (s, e) => PrintEvent("CurrentCellDirtyStateChanged");
            //DataBindingComplete += (s, e) => PrintEvent("DataBindingComplete");
            //DataError += (s, e) => PrintEvent("DataError");
            //DefaultValuesNeeded += (s, e) => PrintEvent("DefaultValuesNeeded");
            //EditingControlShowing += (s, e) => PrintEvent("EditingControlShowing");
            //NewRowNeeded += (s, e) => PrintEvent("NewRowNeeded");
            //RowContextMenuStripChanged += (s, e) => PrintEvent("RowContextMenuStripChanged");
            //RowContextMenuStripNeeded += (s, e) => PrintEvent("RowContextMenuStripNeeded");
            //RowDefaultCellStyleChanged += (s, e) => PrintEvent("RowDefaultCellStyleChanged");
            //RowDirtyStateNeeded += (s, e) => PrintEvent("RowDirtyStateNeeded");
            //RowDividerDoubleClick += (s, e) => PrintEvent("RowDividerDoubleClick");
            //RowDividerHeightChanged += (s, e) => PrintEvent("RowDividerHeightChanged");
            ////RowEnter += (s, e) => PrintEvent("RowEnter");
            //RowErrorTextChanged += (s, e) => PrintEvent("RowErrorTextChanged");
            //RowErrorTextNeeded += (s, e) => PrintEvent("RowErrorTextNeeded");
            //RowHeaderMouseClick += (s, e) => PrintEvent("RowHeaderMouseClick");
            //RowHeaderMouseDoubleClick += (s, e) => PrintEvent("RowHeaderMouseDoubleClick");
            //RowHeaderCellChanged += (s, e) => PrintEvent("RowHeaderCellChanged");
            //RowHeightChanged += (s, e) => PrintEvent("RowHeightChanged");
            ////RowHeightInfoNeeded += (s, e) => PrintEvent("RowHeightInfoNeeded");
            //RowHeightInfoPushed += (s, e) => PrintEvent("RowHeightInfoPushed");
            //RowLeave += (s, e) => PrintEvent("RowLeave");
            //RowMinimumHeightChanged += (s, e) => PrintEvent("RowMinimumHeightChanged");
            //RowPostPaint += (s, e) => PrintEvent("RowPostPaint");
            //RowPrePaint += (s, e) => PrintEvent("RowPrePaint");
            //RowsAdded += (s, e) => PrintEvent("RowsAdded");
            //RowsRemoved += (s, e) => PrintEvent("RowsRemoved");
            //RowStateChanged += (s, e) => PrintEvent("RowStateChanged");
            //RowUnshared += (s, e) => PrintEvent("RowUnshared");
            //RowValidated += (s, e) => PrintEvent("RowValidated");
            //RowValidating += (s, e) => PrintEvent("RowValidating");
            //Scroll += (s, e) => PrintEvent("Scroll");
            //SelectionChanged += (s, e) => PrintEvent("SelectionChanged");
            //SortCompare += (s, e) => PrintEvent("SortCompare");
            //Sorted += (s, e) => PrintEvent("Sorted");
            //StyleChanged += (s, e) => PrintEvent("StyleChanged");
            //UserAddedRow += (s, e) => PrintEvent("UserAddedRow");
            //UserDeletedRow += (s, e) => PrintEvent("UserDeletedRow");
            //UserDeletingRow += (s, e) => PrintEvent("UserDeletingRow");
            //AutoSizeChanged += (s, e) => PrintEvent("AutoSizeChanged");
            //BindingContextChanged += (s, e) => PrintEvent("BindingContextChanged");
            //CausesValidationChanged += (s, e) => PrintEvent("CausesValidationChanged");
            //ClientSizeChanged += (s, e) => PrintEvent("ClientSizeChanged");
            //ContextMenuChanged += (s, e) => PrintEvent("ContextMenuChanged");
            //ContextMenuStripChanged += (s, e) => PrintEvent("ContextMenuStripChanged");
            //CursorChanged += (s, e) => PrintEvent("CursorChanged");
            //DockChanged += (s, e) => PrintEvent("DockChanged");
            //EnabledChanged += (s, e) => PrintEvent("EnabledChanged");
            //LocationChanged += (s, e) => PrintEvent("LocationChanged");
            //MarginChanged += (s, e) => PrintEvent("MarginChanged");
            //RegionChanged += (s, e) => PrintEvent("RegionChanged");
            //RightToLeftChanged += (s, e) => PrintEvent("RightToLeftChanged");
            //SizeChanged += (s, e) => PrintEvent("SizeChanged");
            //TabIndexChanged += (s, e) => PrintEvent("TabIndexChanged");
            //TabStopChanged += (s, e) => PrintEvent("TabStopChanged");
            //VisibleChanged += (s, e) => PrintEvent("VisibleChanged");
            //Click += (s, e) => PrintEvent("Click");
            //ControlAdded += (s, e) => PrintEvent("ControlAdded");
            //ControlRemoved += (s, e) => PrintEvent("ControlRemoved");
            //DragDrop += (s, e) => PrintEvent("DragDrop");
            //DragEnter += (s, e) => PrintEvent("DragEnter");
            //DragOver += (s, e) => PrintEvent("DragOver");
            //DragLeave += (s, e) => PrintEvent("DragLeave");
            //GiveFeedback += (s, e) => PrintEvent("GiveFeedback");
            //HandleCreated += (s, e) => PrintEvent("HandleCreated");
            //HandleDestroyed += (s, e) => PrintEvent("HandleDestroyed");
            //HelpRequested += (s, e) => PrintEvent("HelpRequested");
            //Invalidated += (s, e) => PrintEvent("Invalidated");
            //Paint += (s, e) => PrintEvent("Paint");
            //QueryContinueDrag += (s, e) => PrintEvent("QueryContinueDrag");
            //QueryAccessibilityHelp += (s, e) => PrintEvent("QueryAccessibilityHelp");
            //DoubleClick += (s, e) => PrintEvent("DoubleClick");
            ////Enter += (s, e) => PrintEvent("Enter");
            //GotFocus += (s, e) => PrintEvent("GotFocus");
            //KeyDown += (s, e) => PrintEvent("KeyDown");
            //KeyPress += (s, e) => PrintEvent("KeyPress");
            //KeyUp += (s, e) => PrintEvent("KeyUp");
            //Layout += (s, e) => PrintEvent("Layout");
            //Leave += (s, e) => PrintEvent("Leave");
            //LostFocus += (s, e) => PrintEvent("LostFocus");
            //MouseClick += (s, e) => PrintEvent("MouseClick");
            //MouseDoubleClick += (s, e) => PrintEvent("MouseDoubleClick");
            //MouseCaptureChanged += (s, e) => PrintEvent("MouseCaptureChanged");
            //MouseDown += (s, e) => PrintEvent("MouseDown");
            ////MouseEnter += (s, e) => PrintEvent("MouseEnter");
            ////MouseLeave += (s, e) => PrintEvent("MouseLeave");
            //DpiChangedBeforeParent += (s, e) => PrintEvent("DpiChangedBeforeParent");
            //DpiChangedAfterParent += (s, e) => PrintEvent("DpiChangedAfterParent");
            ////MouseHover += (s, e) => PrintEvent("MouseHover");
            ////MouseMove += (s, e) => PrintEvent("MouseMove");
            //MouseUp += (s, e) => PrintEvent("MouseUp");
            //MouseWheel += (s, e) => PrintEvent("MouseWheel");
            //Move += (s, e) => PrintEvent("Move");
            //PreviewKeyDown += (s, e) => PrintEvent("PreviewKeyDown");
            //Resize += (s, e) => PrintEvent("Resize");
            //ChangeUICues += (s, e) => PrintEvent("ChangeUICues");
            //SystemColorsChanged += (s, e) => PrintEvent("SystemColorsChanged");
            //Validating += (s, e) => PrintEvent("Validating");
            //Validated += (s, e) => PrintEvent("Validated");
            //ParentChanged += (s, e) => PrintEvent("ParentChanged");
            //ImeModeChanged += (s, e) => PrintEvent("ImeModeChanged");
            //Disposed += (s, e) => PrintEvent("Disposed");

        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshRequested?.Invoke(this, EventArgs.Empty);    
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            //Add row numbers to row header
            var rowIdx = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }
        protected override void OnRowValidating(DataGridViewCellCancelEventArgs e)
        {
            base.OnRowValidating(e);
            if (oldRowIndex == e.RowIndex && (UserChangedRowValidated != null || UserChangedRowValidating != null))
            {
                if (IsDifferent(e.RowIndex))
                {
                    UserChangedRowValidating?.Invoke(this, e);
                    if (!e.Cancel)
                    {
                        oldRowIndex = -1;
                        UserChangedRowValidated?.Invoke(this, new DataGridViewRowEventArgs(Rows[e.RowIndex]));
                    }
                }
            }
            oldRowIndex = -1;
        }
        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            base.OnCellBeginEdit(e);
            if (oldRowIndex != e.RowIndex)
                SaveOldRow(e.RowIndex);
        }
        public Func<object, ContextMenu> GetContextMenu { get; set; } = null;
        protected override void OnMouseClick(MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right && GetContextMenu != null)
            //{
            //    var contextrow = HitTest(e.X, e.Y).RowIndex;
            //    if (contextrow >= 0)
            //    {
            //        var rw = Rows[contextrow].DataBoundItem;
            //        var cms = GetContextMenu(rw);
            //        cms.Show(this, new Point(e.X, e.Y));
            //    }
            //}
            base.OnMouseClick(e);
        }
        private List<Object> oldRow;
        private int oldRowIndex = -1;
        private void SaveOldRow(int RowIndex)
        {
            oldRowIndex = RowIndex;
            oldRow = new List<object>();
            foreach (DataGridViewCell cell in Rows[RowIndex].Cells)
                oldRow.Add(cell.Value);
        }
        private bool IsDifferent(int RowIndex)
        {
            if (oldRow == null) return true;
            if (RowIndex != oldRowIndex) return true;

            for (int i = 0; i < ColumnCount; i++)
            {
                if (oldRow[i] == null)
                {
                    if (Rows[RowIndex].Cells[i].Value != null) return true;
                }
                else if (!oldRow[i].Equals(Rows[RowIndex].Cells[i].Value)) return true;
            }

            return false;
        }
    }
}

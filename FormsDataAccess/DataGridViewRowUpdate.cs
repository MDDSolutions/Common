using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{


    // THIS IS DEPRECATED - HAS BEEN MOVED TO MDDWinForms




    // This implementation of the DataGridView uses the BeginCellEdit event to save a copy of the data in a row (in a List<Object>) and then uses the RowValidating event
    // to compare current values to the saved copy.  If differences are found, it fires a custom ChangedRowValidating event allowing the user to cancel and then finally
    // a custom ChangedRowValidated event to indicate that th0a result of a user edit

    /// <summary>
    /// DEPRECATED - Moved to MDDWinForms
    /// </summary>
    public partial class DataGridViewRowUpdate //: DataGridView
    {
        //public event EventHandler<DataGridViewRowEventArgs> ChangedRowValidated;
        //public event EventHandler<DataGridViewCellCancelEventArgs> ChangedRowValidating;
        //public DataGridViewRowUpdate()
        //{
        //    CellBeginEdit += DataGridViewRowUpdate_CellBeginEdit;
        //    RowValidating += DataGridViewRowUpdate_RowValidating;
        //    RowPostPaint += DataGridViewRowUpdate_RowPostPaint;
        //}

        //private void DataGridViewRowUpdate_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    var grid = sender as DataGridView;
        //    var rowIdx = (e.RowIndex + 1).ToString();

        //    var centerFormat = new StringFormat()
        //    {
        //        // right alignment might actually make more sense for numbers
        //        Alignment = StringAlignment.Center,
        //        LineAlignment = StringAlignment.Center
        //    };

        //    var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
        //    e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        //}

        //private void DataGridViewRowUpdate_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        //{
        //    if (oldRowIndex == e.RowIndex && (ChangedRowValidated != null || ChangedRowValidating != null))
        //    {
        //        if (IsDifferent(e.RowIndex))
        //        {
        //            ChangedRowValidating?.Invoke(this, e);
        //            if (!e.Cancel)
        //                ChangedRowValidated?.Invoke(this, new DataGridViewRowEventArgs(Rows[e.RowIndex]));
        //        }
        //    }
        //    oldRowIndex = -1;
        //}
        //private void DataGridViewRowUpdate_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        //{
        //    if (oldRowIndex != e.RowIndex)
        //        SaveOldRow(e.RowIndex);
        //}
        //private List<Object> oldRow;
        //private int oldRowIndex = -1;
        //private void SaveOldRow(int RowIndex)
        //{
        //    oldRowIndex = RowIndex;
        //    oldRow = new List<object>();
        //    foreach (DataGridViewCell cell in Rows[RowIndex].Cells)
        //    {
        //        oldRow.Add(cell.Value);
        //    }
        //}
        //private bool IsDifferent(int RowIndex)
        //{
        //    if (oldRow == null) return true;
        //    if (RowIndex != oldRowIndex) return true;

        //    for (int i = 0; i < ColumnCount; i++)
        //    {
        //        if (oldRow[i] == null)
        //        {
        //            if (Rows[RowIndex].Cells[i].Value != null) return true;
        //        }
        //        else if (!oldRow[i].Equals(Rows[RowIndex].Cells[i].Value)) return true;
        //    }

        //    return false;
        //}
    }
}

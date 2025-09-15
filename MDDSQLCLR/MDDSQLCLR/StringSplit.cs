using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace MDDSQLCLR
{
    public class UserDefinedFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "FillSplitRow",
            TableDefinition = "RowIndex INT, RowData NVARCHAR(MAX)",
            IsDeterministic = true
        )]
        public static IEnumerable SplitCLR(string input, char separator)
        {
            if (input == null || string.IsNullOrWhiteSpace(input))
                return null;

            var parts = input.Split(separator);
            var results = new List<SplitResult>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                results.Add(new SplitResult { RowIndex = i + 1, RowData = parts[i] });
            }
            return results;
        }

        public static void FillSplitRow(object theItem, out SqlInt32 rowIndex, out SqlChars rowData)
        {
            var result = (SplitResult)theItem;
            rowIndex = new SqlInt32(result.RowIndex);
            rowData = new SqlChars(result.RowData);
        }
    }
    public class SplitResult
    {
        public int RowIndex { get; set; }
        public string RowData { get; set; }
    }
}

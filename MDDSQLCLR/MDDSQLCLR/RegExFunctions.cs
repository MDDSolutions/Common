using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace MDDSQLCLR
{
    public class RegexFunctions
    {
        [SqlFunction]
        public static bool RegexContain(SqlString text, SqlString pattern)
        {
            try
            {
                MatchCollection mc = Regex.Matches(text.ToString(), pattern.ToString());
                if (mc.Count > 0)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlBoolean RegexMatch(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull)
                return SqlBoolean.Null;

            return Regex.IsMatch(input.Value, pattern.Value, RegexOptions.IgnoreCase);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString RegexReplace(SqlString input, SqlString pattern, SqlString replacement)
        {
            if (input.IsNull || pattern.IsNull || replacement.IsNull)
                return SqlString.Null;

            return new SqlString(Regex.Replace(input.Value, pattern.Value, replacement.Value, RegexOptions.IgnoreCase));
        }

        [SqlFunction(
            FillRowMethodName = "FillRowSplit",
            TableDefinition = "RowIndex INT, RowData NVARCHAR(MAX)",
            IsDeterministic = true
        )]
        public static IEnumerable RegexSplit(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull)
                return null;

            var split = Regex.Split(input.Value, pattern.Value, RegexOptions.IgnoreCase);
            var result = new List<SplitResult>(split.Length);
            for (int i = 0; i < split.Length; i++)
            {
                result.Add(new SplitResult { RowIndex = i + 1, RowData = split[i] });
            }
            return result;
        }

        [SqlFunction(
            FillRowMethodName = "FillRowMatch", 
            TableDefinition = "Value NVARCHAR(MAX), MatchIndex INT, MatchLength INT")]
        public static IEnumerable RegexMatches(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull)
                return null;

            MatchCollection matches = Regex.Matches(input.Value, pattern.Value, RegexOptions.IgnoreCase);
            var result = new ArrayList();
            foreach (Match match in matches)
            {
                result.Add(new MatchInfo
                {
                    Value = match.Value,
                    Index = match.Index,
                    Length = match.Length
                });
            }
            return result;
        }

        public static void FillRowSplit(object obj, out SqlInt32 rowIndex, out SqlChars rowData)
        {
            //value = new SqlString((string)obj);
            var result = (SplitResult)obj;
            rowIndex = new SqlInt32(result.RowIndex);
            rowData = new SqlChars(result.RowData);
        }

        public static void FillRowMatch(object obj, out SqlString value, out SqlInt32 matchIndex, out SqlInt32 matchLength)
        {
            var matchInfo = (MatchInfo)obj;
            value = new SqlString(matchInfo.Value);
            matchIndex = new SqlInt32(matchInfo.Index);
            matchLength = new SqlInt32(matchInfo.Length);
        }

        private class MatchInfo
        {
            public string Value { get; set; }
            public int Index { get; set; }
            public int Length { get; set; }
        }
    }
}

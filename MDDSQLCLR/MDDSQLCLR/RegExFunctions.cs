using System;
using System.Collections;
using System.Data;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace MDDSQLCLR
{
    public class RegexFunctions
    {
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

        [SqlFunction(FillRowMethodName = "FillRow", TableDefinition = "Value NVARCHAR(MAX)")]
        public static IEnumerable RegexSplit(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull)
                return null;

            return Regex.Split(input.Value, pattern.Value, RegexOptions.IgnoreCase);
        }

        [SqlFunction(FillRowMethodName = "FillRow", TableDefinition = "Value NVARCHAR(MAX)")]
        public static IEnumerable RegexMatches(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull)
                return null;

            MatchCollection matches = Regex.Matches(input.Value, pattern.Value, RegexOptions.IgnoreCase);
            string[] result = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                result[i] = matches[i].Value;
            }
            return result;
        }

        public static void FillRow(object obj, out SqlString value)
        {
            value = new SqlString((string)obj);
        }
    }
}

/*

https://github.com/MDDSolutions/Common/tree/main/MDDSQLCLR

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
    }

}
*/



CREATE ASSEMBLY MDDSQLCLR
FROM 'D:\Temp\MDDSQLCLR.dll'
WITH PERMISSION_SET = SAFE;


CREATE FUNCTION dbo.RegexMatch(@input NVARCHAR(MAX), @pattern NVARCHAR(MAX))
RETURNS BIT
AS EXTERNAL NAME MDDSQLCLR.[MDDSQLCLR.RegexFunctions].RegexMatch;


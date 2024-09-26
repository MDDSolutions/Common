using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace MDDSQLCLR
{
    public class StringSimilarity
    {
        [SqlFunction(IsDeterministic = true, IsPrecise = false)]
        public static SqlDouble LevenshteinSimilarity(SqlString s1, SqlString s2)
        {
            if (s1.IsNull || s2.IsNull)
                return SqlDouble.Null;

            string str1 = s1.Value;
            string str2 = s2.Value;

            int distance = LevenshteinDistance(str1, str2);
            int maxLen = Math.Max(str1.Length, str2.Length);

            if (maxLen == 0) return 1.0; // Both strings are empty

            double similarity = 1.0 - (double)distance / maxLen;
            return new SqlDouble(similarity);
        }

        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
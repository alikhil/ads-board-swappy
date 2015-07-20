using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules
{
    public class SearchModule
    {
        public static int DamerauLevenshteinDistance(string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    return 0;
                }
                else
                {
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            int m = source.Length;
            int n = target.Length;
            int[,] H = new int[m + 2, n + 2];

            int INF = m + n;
            H[0, 0] = INF;
            for (int i = 0; i <= m; i++) { H[i + 1, 1] = i; H[i + 1, 0] = INF; }
            for (int j = 0; j <= n; j++) { H[1, j + 1] = j; H[0, j + 1] = INF; }

            SortedDictionary<char, int> sd = new SortedDictionary<char, int>();
            foreach (char Letter in (source + target))
            {
                if (!sd.ContainsKey(Letter))
                    sd.Add(Letter, 0);
            }

            for (int i = 1; i <= m; i++)
            {
                int DB = 0;
                for (int j = 1; j <= n; j++)
                {
                    int i1 = sd[target[j - 1]];
                    int j1 = DB;

                    if (source[i - 1] == target[j - 1])
                    {
                        H[i + 1, j + 1] = H[i, j];
                        DB = j;
                    }
                    else
                    {
                        H[i + 1, j + 1] = Math.Min(H[i, j], Math.Min(H[i + 1, j], H[i, j + 1])) + 1;
                    }

                    H[i + 1, j + 1] = Math.Min(H[i + 1, j + 1], H[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[source[i - 1]] = i;
            }

            return H[m + 1, n + 1];
        }

        public static bool AInB(string a, string b)
        {
            int k = 3, ml = 9999, minLev = 99999;
            a = a.ToLower();
            b = b.ToLower();
            string tot = b;
            for (int i = 0; i <= tot.Length - a.Length; i++)
            {
                ml = DamerauLevenshteinDistance(tot.Substring(i, a.Length), a);
                minLev = ml < minLev ? ml : minLev;
            }
            return minLev <= k;
        }
    }
}
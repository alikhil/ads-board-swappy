using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swappy_V2.Classes;

namespace Swappy_V2.Modules
{
    /// <summary>
    /// Модуль поиска объявлений
    /// </summary>
    public static class SearchModule
    {
        /// <summary>
        /// Минимальная допустимый коэфицент совпадения заголовка объявления
        /// со строкой поиска
        /// </summary>
        public static double Fuzzyness = 0.7;

        /// <summary>
        /// Поиск объявлений
        /// </summary>
        /// <param name="request">Строка поиска</param>
        /// <param name="ar">Список объектов типа Searchable в котором надо искать</param>
        /// <returns>Результаты поиска</returns>
        public static SearchRequest FindOut(string request, IEnumerable<Searchable> ar)
        {
            SearchRequest req = new SearchRequest { Request = request };
            int cnt = 0;
            var FullMatch = new Dictionary<int, KeyValuePair<Searchable, double>>();
            var FullSubstringMatch = new Dictionary<int, KeyValuePair<Searchable, double>>();
            var IncompleteMatch = new Dictionary<int, KeyValuePair<Searchable, double>>();
            foreach (var src in ar)
            {
                var source = src.SearchBy();
                if (request.ToLower() == source.ToLower())
                    FullMatch.Add(cnt++, new KeyValuePair<Searchable, double>(src, 1));
                else
                {
                    double fuz = GetFuzze(request, source);
                    if (fuz == 1.0d)
                        FullSubstringMatch.Add(cnt++, new KeyValuePair<Searchable, double>(src, 1 - fuz));
                    else
                        if (fuz > Fuzzyness)
                            IncompleteMatch.Add(cnt++, new KeyValuePair<Searchable, double>(src, 1 - fuz));

                }
            }
            FullMatch.OrderBy(x => x.Value.Key);
            FullSubstringMatch.OrderBy(x => x.Value.Key);
            IncompleteMatch.OrderBy(x => x.Value.Value);

            req.FullMatch = (from a in FullMatch select a.Value).ToList<KeyValuePair<Searchable, double>>(FullMatch.Count);
            req.FullSubstringMatch = (from a in FullSubstringMatch select a.Value).ToList<KeyValuePair<Searchable, double>>(FullSubstringMatch.Count);
            req.IncompleteMatch = (from a in IncompleteMatch select a.Value).ToList<KeyValuePair<Searchable, double>>(IncompleteMatch.Count);

            return req;
        }
        public static double GetFuzze(string a, string b)
        {
            a = a.ToLower();
            b = b.ToLower();
            double maxF = 0, fuz;
            for (int i = 0; i <= b.Length - a.Length; i++)
            {
                fuz = FuzzyBContainsA(b.Substring(i, a.Length), a);
                if (fuz > maxF)
                    maxF = fuz;
            }
            return maxF;
        }

        public static bool BContainsA(string a, string b)
        {
            a = a.ToLower();
            b = b.ToLower();
            string tot = b;
            for (int i = 0; i <= tot.Length - a.Length; i++)
            {
                if (FuzzyBContainsA(b.Substring(i, a.Length), a) > Fuzzyness)
                    return true;
            }

            return false;
        }

        public static bool ContainsA(this string b, string a)
        {
            a = a.ToLower();
            b = b.ToLower();
            string tot = b;
            for (int i = 0; i <= tot.Length - a.Length; i++)
            {
                if (FuzzyBContainsA(b.Substring(i, a.Length), a) > Fuzzyness)
                    return true;
            }

            return false;
        }

        public static double FuzzyBContainsA(string a, string b)
        {
            int levenshteinDistance = LevenshteinDistance(a, b);
            int length = Math.Max(a.Length, b.Length);
            return 1.0 - (double)levenshteinDistance / length;
        }

        public static int LevenshteinDistance(string src, string dest)
        {
            int[,] d = new int[src.Length + 1, dest.Length + 1];
            int i, j, cost, deleteCost = 1, insertCost = 1, subtCost = 1;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {

                    if (str1[i - 1] == str2[j - 1])
                        cost = 0;
                    else
                        cost = subtCost;

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + deleteCost,              // Deletion
                            Math.Min(
                                d[i, j - 1] + insertCost,          // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] ==
                        str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }
    }
}
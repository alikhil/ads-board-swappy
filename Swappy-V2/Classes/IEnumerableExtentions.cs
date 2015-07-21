using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public static class IEnumerableExtentions
    {
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            var list = new List<TSource>(count);
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
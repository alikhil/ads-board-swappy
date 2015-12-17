using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes.Extensions
{
    /// <summary>
    /// Extension class for IEnumerable
    /// </summary>
    public static class IEnumerableExtentions
    {
        /// <summary>
        /// Better ToList because of known count of elements
        /// </summary>
        /// <typeparam name="TSource">Sype of storing elements in IEnumerable</typeparam>
        /// <param name="source">Sourse list</param>
        /// <param name="count">Count of elements in ienumerable</param>
        /// <returns>List of "TSourse" objects</returns>
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
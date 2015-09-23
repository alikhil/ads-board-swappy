using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    /// <summary>
    /// Класс дополнений для IEnumerable
    /// </summary>
    public static class IEnumerableExtentions
    {
        /// <summary>
        /// Ускоренный ToList благодаря явной передаче кол-во элементов в списке
        /// </summary>
        /// <typeparam name="TSource">Тип данный хранимый в IEnumerable</typeparam>
        /// <param name="source">Исходный список</param>
        /// <param name="count">Кол-во элементов в IEnumerable</param>
        /// <returns>Список объектов "TSourse"</returns>
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
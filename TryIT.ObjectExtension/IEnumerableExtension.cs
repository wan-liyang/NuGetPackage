using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// Extension method for <see cref="IEnumerable{T}"/> item
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// indicator whether <paramref name="list"/> is null or empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable<object> list)
        {
            if (list == null || list.Count() == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// check whether string list contains provided value, default ignore case, return false if value is null
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsContains(this IEnumerable<string> list, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (list == null || list.ToList().Count == 0 || value == null)
            {
                return false;
            }
            return list.Any(p => p.IsEquals(value, comparisonType));
        }

        /// <summary>
        /// distict IEnumerable object by specific property
        /// <para>list.DistinctBy(p => p.Id)</para>
        /// <para>list.DistinctBy(p => new { p.Id, p.Name })</para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Convert list to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public static DataTable ToDataTable<T>(this T item)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);

            return dataTable;
        }
    }
}

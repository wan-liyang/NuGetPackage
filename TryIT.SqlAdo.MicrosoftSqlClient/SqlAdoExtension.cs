using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient
{
    /// <summary>
    /// extension method
    /// </summary>
    public class SqlAdoExtension
    {
        /// <summary>
        /// convert value to <see cref="DBNull.Value"/> if <paramref name="value"/> is NULL or Empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;
        }

        /// <summary>
        /// convert value to <see cref="DBNull.Value"/> if <paramref name="value"/> is NULL or exists in <paramref name="nullValues"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullValues"></param>
        /// <returns></returns>
        public static object ToDBNull(Nullable<int> value, params int[] nullValues)
        {
            if (value.HasValue)
            {
                if (nullValues == null || nullValues.Length == 0 || !nullValues.Contains(value.Value))
                {
                    return value.Value;
                }
            }
            return DBNull.Value;
        }

        /// <summary>
        /// convert value to <see cref="DBNull.Value"/> if <paramref name="value"/> is NULL or exists in <paramref name="nullValues"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullValues"></param>
        /// <returns></returns>
        public static object ToDBNull(Nullable<decimal> value, params decimal[] nullValues)
        {
            if (value.HasValue)
            {
                if (nullValues == null || nullValues.Length == 0 || !nullValues.Contains(value.Value))
                {
                    return value.Value;
                }
            }
            return DBNull.Value;
        }

        /// <summary>
        /// return DBNull.Value if <paramref name="value"/> is NULL or year is 1900
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBNull(Nullable<DateTime> value)
        {
            if (value.HasValue && value.Value.Year != 1900)
            {
                return value.Value;
            }
            return DBNull.Value;
        }
    }
}

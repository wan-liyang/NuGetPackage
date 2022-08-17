﻿using System;
using System.Linq;

namespace TryIT.SqlAdoService
{
    /// <summary>
    /// extension method for SQL ADO, e.g. parse value to <see cref="DBNull.Value"/>
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


        /// <summary>
        /// Convert object value to specific type value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objValue"></param>
        /// <returns></returns>
        internal static T ConvertValue<T>(object objValue)
        {
            T value = default(T);
            if (objValue != null && objValue != DBNull.Value)
            {
                Type type = typeof(T);
                try
                {
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                            if (string.IsNullOrEmpty(objValue.ToString()))
                            {
                                return value;
                            }
                            else if (objValue.GetType().FullName.ToUpper() == "SYSTEM.BOOLEAN")
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            else if (objValue.ToString().ToUpper() == "Y"
                                || objValue.ToString().ToUpper() == "YES"
                                || objValue.ToString().ToUpper() == "ON")
                            {
                                value = (T)Convert.ChangeType(true, type);
                            }
                            else if (objValue.ToString().ToUpper() == "N"
                                || objValue.ToString().ToUpper() == "NO"
                                || objValue.ToString().ToUpper() == "OFF")
                            {
                                value = (T)Convert.ChangeType(false, type);
                            }
                            else
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            break;
                        case TypeCode.Int32:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            if (string.IsNullOrEmpty(objValue.ToString()))
                            {
                                return value;
                            }
                            else
                            {
                                value = (T)Convert.ChangeType(objValue, type);
                            }
                            break;
                        default:
                            value = (T)Convert.ChangeType(objValue, type);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return value;
        }
    }
}

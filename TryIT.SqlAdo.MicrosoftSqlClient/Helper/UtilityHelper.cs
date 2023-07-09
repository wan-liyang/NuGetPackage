using System;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    internal class UtilityHelper
    {
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
                catch
                {
                    throw;
                }
            }
            return value;
        }
    }
}

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// helper for convert value
    /// </summary>
    public static class SqlHelper
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

        /// <summary>
        /// convert value to <see cref="DBNull.Value"/> if <paramref name="value"/> is NULL or Empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullIfEmpty">indicate return DBNull if <paramref name="value"/> is empty</param>
        /// <returns></returns>
        public static object ToDBNull(this string value, bool nullIfEmpty = false)
        {
            if (!nullIfEmpty)
            {
                if (value == string.Empty)
                {
                    return value;
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;
        }

        /// <summary>
        /// convert value to <see cref="DBNull.Value"/> if <paramref name="value"/> is NULL, Empty or exists in <paramref name="nullValues"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullValues"></param>
        /// <returns></returns>
        public static object ToDBNull(this string value, params string[] nullValues)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (nullValues == null || nullValues.Length == 0 || !nullValues.Contains(value))
                {
                    return value;
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
        public static object ToDBNull(this Nullable<int> value, params int[] nullValues)
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
        public static object ToDBNull(this Nullable<decimal> value, params decimal[] nullValues)
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
        public static object ToDBNull(this Nullable<DateTime> value)
        {
            if (value.HasValue && value.Value.Year != 1900)
            {
                return value.Value;
            }
            return DBNull.Value;
        }

        /// <summary>
        /// get a Guid that doesn't contain any numbers and dash
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            // before: 51e3aaa4-6ff6-475a-8f0f-78cac597b6c3
            // after: FBVDRRREGWWGEHFRIWAWHITRTFJHSGTD
            return string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17))).ToUpper();
        }

        /// <summary>
        /// warp column, Column1 to [Column1]
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string SqlWarpColumn(string column)
        {
            return column.StartsWith("[") ? column : $"[{column}]";
        }

        /// <summary>
        /// warp column, Column1 to [Column1]
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<string> SqlWarpColumn(IEnumerable<string> columns)
        {
            List<string> strings = new List<string>();
            foreach (var column in columns)
            {
                strings.Add(column.StartsWith("[") ? column : $"[{column}]");
            }
            return strings;
        }

        /// <summary>
        /// clean parameter name, remove the space, remove [ ]
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string SqlParamName(string param)
        {
            return param.Replace(" ", "").Replace("[", "").Replace("]", "");
        }



        /*
            https://learn.microsoft.com/en-us/sql/relational-databases/security/encryption/develop-using-always-encrypted-with-net-framework-data-provider?view=sql-server-2017#inserting-data-example


        1. The data type of the parameter targeting the SSN column is set to an ANSI (non-Unicode) string, which maps to the char/varchar SQL Server data type. If the type of the parameter was set to a Unicode string (String), which maps to nchar/nvarchar, the query would fail, as Always Encrypted doesn't support conversions from encrypted nchar/nvarchar values to encrypted char/varchar values. See SQL Server Data Type Mappings for information about the data type mappings.\

        2. The data type of the parameter inserted into the BirthDate column is explicitly set to the target SQL Server data type using SqlParameter.SqlDbType Property, instead of relying on the implicit mapping of .NET types to SQL Server data types applied when using SqlParameter.DbType Property. By default, DateTime Structure maps to the datetime SQL Server data type. As the data type of the BirthDate column is date and Always Encrypted doesn't support a conversion of encrypted datetime values to encrypted date values, using the default mapping would result in an error.

        */

        /// <summary>
        /// https://learn.microsoft.com/en-us/sql/relational-databases/security/encryption/develop-using-always-encrypted-with-net-framework-data-provider?view=sql-server-2017#inserting-data-example
        /// </summary>
        /// <param name="paramName">parameter name without @</param>
        /// <param name="paramValue"></param>
        /// <param name="alwaysEncryptedColumn">always encrypted column information for the target column which going to update</param>
        /// <returns></returns>
        public static SqlParameter GetParameter(string paramName, object paramValue, AlwaysEncryptedColumn alwaysEncryptedColumn = null)
        {
            SqlParameter sqlParameter = new SqlParameter();

            sqlParameter.ParameterName = $"@{SqlHelper.SqlParamName(paramName)}";
            sqlParameter.Direction = ParameterDirection.Input;
            sqlParameter.Value = paramValue;

            if (alwaysEncryptedColumn != null)
            {
                switch (alwaysEncryptedColumn.ColumnType.ToUpper())
                {
                    case "VARCHAR":
                        sqlParameter.DbType = DbType.AnsiStringFixedLength;
                        sqlParameter.Size = alwaysEncryptedColumn.ColumnMaxLength;
                        break;
                    case "NVARCHAR":
                        sqlParameter.DbType = DbType.String;
                        sqlParameter.Size = alwaysEncryptedColumn.ColumnMaxLength;
                        break;
                    case "DATE":
                        sqlParameter.SqlDbType = SqlDbType.Date;
                        break;
                    case "DATETIME":
                        sqlParameter.SqlDbType = SqlDbType.DateTime;
                        break;
                    case "DECIMAL":
                        sqlParameter.SqlDbType = SqlDbType.Decimal;
                        break;
                    case "INT":
                        sqlParameter.SqlDbType = SqlDbType.Int;
                        break;
                    default:
                        break;
                }
            }

            return sqlParameter;
        }
    }
}

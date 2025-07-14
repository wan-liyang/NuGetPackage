using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// generate sql script based on entity
    /// </summary>
    public static class SqlGenerator
    {
        /// <summary>
        /// Gets all public instance properties of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to reflect</typeparam>
        /// <returns></returns>
        private static PropertyInfo[] GetPropertyInfos<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Gets the public instance properties of type <typeparamref name="T"/> that are decorated with the specified attribute
        /// </summary>
        /// <typeparam name="T">The type to reflect</typeparam>
        /// <param name="attributeType">The type of the attribute to filter by</param>
        /// <returns></returns>
        private static PropertyInfo[] GetPropertyInfosWithAttribute<T>(Type attributeType)
        {
            var properties = GetPropertyInfos<T>();

            return properties?.Where(p => p.GetCustomAttributes(attributeType, true).Any()).ToArray();
        }

        /// <summary>
        /// generate select sql script based on <paramref name="entity"/>, if <paramref name="entity"/> has property with <see cref="KeyAttribute"/>, that property will be use as where condition also
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static (string Sql, List<SqlParameter> Parameters) GenerateSelectSql<T>(string tableName, T entity)
        {
            tableName = SqlHelper.SqlWarpTable(tableName);

            var columns_values_select = new StringBuilder();
            var columns_values_where = new StringBuilder();
            var parameters = new List<SqlParameter>();

            var properties = GetPropertyInfos<T>();
            foreach (var prop in properties)
            {
                string columnName = prop.Name;

                columns_values_select.Append($"{SqlHelper.SqlWarpColumn(columnName)},");

                bool isKey = prop.GetCustomAttribute(typeof(KeyAttribute)) != null;

                if (isKey)
                {
                    // sql parameters
                    object value = GetDbValueForProperty(prop, entity);

                    if (value == DBNull.Value)
                    {
                        throw new ArgumentNullException($"the key [{columnName}] must not be null");
                    }

                    parameters.Add(new SqlParameter($"@{columnName}", value));
                    columns_values_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
                }
            }

            string sql = $"SELECT {columns_values_select.ToString().TrimEnd(',')} FROM {tableName} WITH(NOLOCK)";

            if (columns_values_where.Length > 0)
            {
                string sqlWhere = TrimEnd(columns_values_where.ToString(), " AND ");
                sql += $" WHERE {sqlWhere}";
            }

            return (sql, parameters);
        }

        /// <summary>
        /// generate update sql script based on <paramref name="entity"/>, the property from <paramref name="entity"/> must has at least one property with <see cref="KeyAttribute"/> to use as where condition, also must at least one property without <see cref="KeyAttribute"/> to perform update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static (string Sql, List<SqlParameter> Parameters) GenerateUpdateSql<T>(string tableName, T entity)
        {
            var keyProperties = GetPropertyInfosWithAttribute<T>(typeof(KeyAttribute));
            if (keyProperties == null || !keyProperties.Any())
            {
                throw new ArgumentNullException($"{typeof(KeyAttribute)} is missing in entity {typeof(T)}");
            }

            var nonKeyProperties = GetPropertyInfosWithAttribute<T>(typeof(KeyAttribute));
            if (nonKeyProperties == null || !nonKeyProperties.Any())
            {
                throw new ArgumentNullException($"no property to update in entity {typeof(T)}");
            }

            var columns_values_update = new StringBuilder();
            var columns_values_update_where = new StringBuilder();
            var parameters = new List<SqlParameter>();

            var properties = GetPropertyInfos<T>();
            foreach (var prop in properties)
            {
                string columnName = prop.Name;

                // sql parameters
                object value = GetDbValueForProperty(prop, entity);
                parameters.Add(new SqlParameter($"@{columnName}", value));

                // sql update
                bool isKey = prop.GetCustomAttribute(typeof(KeyAttribute)) != null;

                if (isKey)
                {
                    if (value == DBNull.Value)
                    {
                        throw new ArgumentNullException($"the key [{columnName}] must not be null");
                    }

                    columns_values_update_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
                }
                else
                {
                    // indicate whether the field can be update
                    bool isNotUpdatable = CheckPropertyAttribute(prop, typeof(SqlUpdatableAttribute), "IsUpdatable", false);

                    if (!isNotUpdatable)
                    {
                        columns_values_update.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName},");
                    }
                }
            }

            string sqlWhere = TrimEnd(columns_values_update_where.ToString(), " AND ");
            string sql = $"UPDATE {tableName} SET {columns_values_update.ToString().TrimEnd(',')} WHERE {sqlWhere}";

            return (sql, parameters);
        }

        /// <summary>
        /// genereate delete sql script based on <paramref name="entity"/>, the entity must has at least one property with <see cref="KeyAttribute"/> to use as where condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (string Sql, List<SqlParameter> Parameters) GenerateDeleteSql<T>(string tableName, T entity)
        {
            var keyProperties = GetPropertyInfosWithAttribute<T>(typeof(KeyAttribute));
            if (keyProperties == null || !keyProperties.Any())
            {
                throw new ArgumentNullException($"{typeof(KeyAttribute)} is missing in entity {typeof(T)}");
            }

            var parameters = new List<SqlParameter>();
            var columns_values_update_where = new StringBuilder();
            foreach (var prop in keyProperties)
            {
                string columnName = prop.Name;

                // sql parameters
                object value = GetDbValueForProperty(prop, entity);

                if (value == DBNull.Value)
                {
                    throw new ArgumentNullException($"the key [{columnName}] must not be null");
                }

                parameters.Add(new SqlParameter($"@{columnName}", value));

                columns_values_update_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
            }

            string sqlWhere = TrimEnd(columns_values_update_where.ToString(), " AND ");
            string sql = $"DELETE FROM {tableName} WHERE {sqlWhere}";

            return (sql, parameters);
        }

        /// <summary>
        /// generate sql script to insert or update record in table, if the entity has property with <see cref="KeyAttribute"/>, the script will perform update if record exists, otherwise insert new record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (string Sql, List<SqlParameter> Parameters) GenerateInsertUpdateSql<T>(string tableName, T entity)
        {
            tableName = SqlHelper.SqlWarpTable(tableName);

            var properties = GetPropertyInfos<T>();

            bool hasKey = false;

            var columns_insert = new StringBuilder();
            var values_insert = new StringBuilder();
            var parameters = new List<SqlParameter>();

            var columns_values_update = new StringBuilder();
            var columns_values_update_where = new StringBuilder();

            foreach (var prop in properties)
            {
                string columnName = prop.Name;

                // sql parameters
                object value = GetDbValueForProperty(prop, entity);
                parameters.Add(new SqlParameter($"@{columnName}", value));

                // sql insert
                if (columns_insert.Length > 0)
                {
                    columns_insert.Append(", ");
                    values_insert.Append(", ");
                }

                columns_insert.Append(SqlHelper.SqlWarpColumn(columnName));
                values_insert.Append($"@{columnName}");


                // sql update
                bool isKey = prop.GetCustomAttribute(typeof(KeyAttribute)) != null;

                if (isKey)
                {
                    if (value == DBNull.Value)
                    {
                        throw new ArgumentNullException($"the key [{columnName}] must not be null");
                    }

                    hasKey = true;
                    columns_values_update_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
                }
                else
                {
                    // indicate whether the field can be update
                    bool isNotUpdatable = CheckPropertyAttribute(prop, typeof(SqlUpdatableAttribute), "IsUpdatable", false);

                    if (!isNotUpdatable)
                    {
                        columns_values_update.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName},");
                    }
                }
            }

            string sql = string.Empty;
            string sqlWhere = TrimEnd(columns_values_update_where.ToString(), " AND ");
            string sqlUpdate = $"UPDATE {tableName} SET {columns_values_update.ToString().TrimEnd(',')} WHERE {sqlWhere}";
            string sqlInsert = $"INSERT INTO {tableName} ({columns_insert}) VALUES ({values_insert})";

            if (hasKey)
            {
                sql = $"IF EXISTS (SELECT 1 FROM {tableName} WITH(NOLOCK) WHERE {sqlWhere}) BEGIN {sqlUpdate} END ELSE BEGIN {sqlInsert} END;";
            }
            else
            {
                sql = sqlInsert;
            }

            return (sql, parameters);
        }

        /// <summary>
        /// generate sql script to check whether record exists in table, the script will return 1 if record exists, otherwise empty table will be return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static (string Sql, List<SqlParameter> Parameters) GenerateExistsSql<T>(string tableName, T entity)
        {
            var keyProperties = GetPropertyInfosWithAttribute<T>(typeof(KeyAttribute));
            if (keyProperties == null || !keyProperties.Any())
            {
                throw new ArgumentNullException($"{typeof(KeyAttribute)} is missing in entity {typeof(T)}");
            }

            tableName = SqlHelper.SqlWarpTable(tableName);
            var parameters = new List<SqlParameter>();
            var columns_values_update_where = new StringBuilder();
            foreach (var prop in keyProperties)
            {
                string columnName = prop.Name;

                // sql parameters
                object value = GetDbValueForProperty(prop, entity);

                if (value == DBNull.Value)
                {
                    throw new ArgumentNullException($"the key [{columnName}] must not be null");
                }

                parameters.Add(new SqlParameter($"@{columnName}", value));

                columns_values_update_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
            }

            string sqlWhere = TrimEnd(columns_values_update_where.ToString(), " AND ");
            string sql = $"SELECT TOP 1 1 FROM {tableName} WHERE {sqlWhere}";

            return (sql, parameters);
        }

        /// <summary>
        /// generate sql script to check whether record exists in table, the script will return 1 if record exists, otherwise empty table will be return
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filterKeyAndValue">the filter value must not be null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static (string Sql, List<SqlParameter> Parameters) GenerateExistsSql(string tableName, Dictionary<string, object> filterKeyAndValue)
        {
            if (filterKeyAndValue == null || !filterKeyAndValue.Any())
            {
                throw new ArgumentNullException(nameof(filterKeyAndValue));
            }

            if (filterKeyAndValue.Any(p => p.Value == null))
            {
                throw new ArgumentException($"{nameof(filterKeyAndValue)} invalid, the value must not be null");
            }

            tableName = SqlHelper.SqlWarpTable(tableName);
            var parameters = new List<SqlParameter>();
            var columns_values_update_where = new StringBuilder();
            foreach (var item in filterKeyAndValue)
            {
                string columnName = item.Key;

                // sql parameters
                parameters.Add(new SqlParameter($"@{columnName}", item.Value));
                columns_values_update_where.Append($"{SqlHelper.SqlWarpColumn(columnName)} = @{columnName} AND ");
            }

            string sqlWhere = TrimEnd(columns_values_update_where.ToString(), " AND ");
            string sql = $"IF EXISTS (SELECT 1 FROM {tableName} WITH(NOLOCK) WHERE {sqlWhere}) SELECT 1";

            return (sql, parameters);
        }

        private static string TrimEnd(string source, string value)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }

        /// <summary>
        /// check whether <paramref name="objectProperty"/> has attribute <paramref name="attributeType"/> with attribute's property <paramref name="attProp"/> is value <paramref name="attPropValue"/>
        /// </summary>
        /// <param name="objectProperty"></param>
        /// <param name="attributeType"></param>
        /// <param name="attProp"></param>
        /// <param name="attPropValue"></param>
        /// <returns></returns>
        private static bool CheckPropertyAttribute(PropertyInfo objectProperty, Type attributeType, string attProp, object attPropValue)
        {
            var att = objectProperty.GetCustomAttributes(attributeType, true);
            if (att == null || !att.Any()) return false;

            var attProperty = attributeType.GetProperty(attProp);
            if (attProperty == null) return false;

            return object.Equals(attProperty.GetValue(att[0], null), attPropValue);
        }

        /// <summary>
        /// get db value for a property, below type of value will return <see cref="DBNull.Value"/>
        /// <para>null value</para>
        /// <para>nullable datetime property with <see cref="DateTime.MinValue"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectProperty"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static object GetDbValueForProperty<T>(PropertyInfo objectProperty, T entity)
        {
            var value = objectProperty.GetValue(entity);

            if (value == null)
                return DBNull.Value; // Convert all null values to DBNull.Value

            Type propertyType = objectProperty.PropertyType;

            // Check for specific types with custom logic
            if (propertyType.IsNullableOf<DateTime>() && (DateTime)value == DateTime.MinValue)
            {
                return DBNull.Value; // Convert DateTime.MinValue to DBNull.Value
            }

            return value;
        }

        private static bool IsNullableOf<T>(this Type type)
        {
            return Nullable.GetUnderlyingType(type) == typeof(T);
        }
    }
}

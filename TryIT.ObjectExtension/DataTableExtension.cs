using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TryIT.ObjectExtension
{
    /// <summary>
    /// extension method for <see cref="System.Data.DataTable"/>
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        /// Indicates whether the specified DataTable is null or has not row.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>true: table is null or has not row, false: table is not null and has row</returns>
        public static bool IsNullOrEmpty(this DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0;
        }

        /// <summary>
        /// convert specified column to string list, return empty list if dtSource or columnName invalid.
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="columnName"></param>
        /// <param name="ignoreBlank">whether ignore blank or null</param>
        /// <returns></returns>
        public static IList<string> ToList(this DataTable dtSource, string columnName, bool ignoreBlank = true)
        {
            List<string> retList = new List<string>();
            if (null != dtSource && dtSource.Rows.Count > 0
                && !string.IsNullOrEmpty(columnName)
                && dtSource.Columns.Cast<DataColumn>().Select(p => p.ColumnName).IsContains(columnName))
            {
                string tmpValue = string.Empty;
                foreach (DataRow row in dtSource.Rows)
                {
                    tmpValue = row[columnName]?.ToString();
                    if (ignoreBlank && string.IsNullOrEmpty(tmpValue))
                    {
                        continue;
                    }
                    retList.Add(tmpValue);
                }
            }
            return retList;
        }

        /// <summary>
        /// convert datatable to object list, only load datatable columns matched with object property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtSource"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dtSource) where T : class, new()
        {
            List<T> list = new List<T>();

            var props = (new T()).GetType().GetProperties();

            foreach (DataRow row in dtSource.Rows)
            {
                T obj = new T();
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    string colName = dtSource.Columns[i].ColumnName;
                    var prop = props.Where(p => p.Name.IsEquals(colName)).FirstOrDefault();
                    if (prop != null)
                    {
                        if (row[colName] != DBNull.Value)
                        {
                            SetValue(obj, prop, row[colName]);
                        }
                    }
                }
                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// convert <paramref name="dtSource"/> to list of <typeparamref name="T"/>, based on <paramref name="keyValues"/> mapping,
        /// <para>Keys: is Source Column</para>
        /// <para>Values: is Target Entity Property</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtSource"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dtSource, Dictionary<string, string> keyValues) where T : class, new()
        {
            List<T> list = new List<T>();

            List<string> columns = keyValues.Keys.ToList();
            List<string> properties = keyValues.Values.ToList();

            if (dtSource != null && dtSource.Rows.Count > 0
                && columns != null && columns.Count() > 0
                && properties != null && properties.Count() > 0)
            {
                var props = (new T()).GetType().GetProperties();

                foreach (DataRow row in dtSource.Rows)
                {
                    T obj = new T();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        string colName = columns[i];
                        if (dtSource.Columns.Contains(colName))
                        {
                            // if get respective property and assign value
                            string propName = properties[i];
                            var prop = props.Where(p => p.Name.IsEquals(propName)).FirstOrDefault();
                            if (prop != null)
                            {
                                if (row[colName] != DBNull.Value)
                                {
                                    SetValue(obj, prop, row[colName]);
                                }
                            }
                        }
                    }
                    list.Add(obj);
                }
            }
            return list;
        }

        private static void SetValue(Object obj, PropertyInfo prop, object value)
        {
            if (prop.PropertyType.IsGenericType && prop.PropertyType.Name.Contains("Nullable"))
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    prop.SetValue(obj, ConvertValueToType(value, Nullable.GetUnderlyingType(prop.PropertyType)));
                }
            }
            else if (prop.PropertyType.IsEnum)
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    prop.SetValue(obj, Enum.Parse(prop.PropertyType, value.ToString()));
                }
            }
            else
            {
                prop.SetValue(obj, ConvertValueToType(value, prop.PropertyType), null);
            }
        }


        private static object ConvertValueToType(object value, Type type)
        {
            try
            {
                if (type == typeof(System.Data.SqlTypes.SqlBinary))
                {
                    return Encoding.UTF8.GetBytes(value.ToString());
                }

                // guid to string, need this special handle
                if (value is Guid && Type.GetTypeCode(type) == TypeCode.String)
                {
                    return value.ToString();
                }

                // string to guid, need this special handle
                if (value is String && type.FullName == "System.Guid")
                {
                    return Guid.Parse(value.ToString());
                }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        if (value == null)
                        {
                            return Convert.ChangeType(false, type);
                        }
                        else if (value.ToString().ToLower() == "y" || value.ToString().ToLower() == "yes")
                        {
                            return Convert.ChangeType(true, type);
                        }
                        else if (value.ToString().ToLower() == "n" || value.ToString().ToLower() == "no")
                        {
                            return Convert.ChangeType(false, type);
                        }
                        else
                        {
                            return Convert.ChangeType(value, type);
                        }
                    default:
                        return Convert.ChangeType(value, type);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
         <table style="">
            <thead>
                <tr>
                    <th style="">Col1</th>
                    <th>Col2</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="">Row1-Col1</td>
                    <td>Row1-Col2</td>
                </tr>
                <tr>
                    <td>Row2-Col1</td>
                    <td>Row2-Col2</td>
                </tr>
            </tbody>
        </table>
        */

        /// <summary>
        /// style value for convert DataTable to HtmlString
        /// </summary>
        public class ToHtmlString_TableStyle
        {
            /// <summary>
            /// Table format
            /// </summary>
            public string Table { get; set; }
            /// <summary>
            /// Th format
            /// </summary>
            public string Th { get; set; }
            /// <summary>
            /// Td format
            /// </summary>
            public string Td { get; set; }
        }

        /// <summary>
        /// delegate function for customize cell format
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public delegate string Delegate_TdFormat(DataTable dataTable, int row, int col);

        /// <summary>
        /// convert DataTable to HtmlString
        /// <para>\r\n &amp; \n will be replace to <br/> </para>
        /// <para>return html table string: &lt;table style=&quot;&quot;&gt;&lt;thead&gt;&lt;tr&gt;&lt;th style=&quot;&quot;&gt;Col1&lt;/th&gt;&lt;th&gt;Col2&lt;/th&gt;&lt;/tr&gt;&lt;/thead&gt;&lt;tbody&gt;&lt;tr&gt;&lt;td style=&quot;&quot;&gt;Row1-Col1&lt;/td&gt;&lt;td&gt;Row1-Col2&lt;/td&gt;&lt;/tr&gt;&lt;tr&gt;&lt;td&gt;Row2-Col1&lt;/td&gt;&lt;td&gt;Row2-Col2&lt;/td&gt;&lt;/tr&gt;&lt;/tbody&gt;&lt;/table&gt;</para>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="style">table style for Table / Tr / Td, default will set "border-collapse: collapse; border: 1px solid;"</param>
        /// <param name="tdFormatFunc">delegate function to allow customize format Td content when Td is not DBNull, e.g. add hyperlink to Td
        /// <para>string Func(DataTable dataTable, int row, int col)</para>
        /// </param>
        /// <param name="columns">columns to include in html, default null to include all columns</param>
        /// <param name="ignoreHeader">indicate to ignore table header when generate html string</param>
        /// <returns></returns>
        public static string ToHtmlString(this DataTable dt, ToHtmlString_TableStyle style = null, Delegate_TdFormat tdFormatFunc = null, IEnumerable<string> columns = null, bool ignoreHeader = false)
        {
            string style_table;
            string style_th;
            string style_td;

            // assign default style if not provide style
            if (style == null)
            {
                style = new DataTableExtension.ToHtmlString_TableStyle
                {
                    Table = "border-collapse: collapse;border: 1px solid;",
                    Th = "border: 1px solid;padding: 5px;",
                    Td = "border: 1px solid;padding: 5px;"
                };
            }

            style_table = style == null || string.IsNullOrEmpty(style.Table) ? "" : style.Table;
            style_th = style == null || string.IsNullOrEmpty(style.Th) ? "" : style.Th;
            style_td = style == null || string.IsNullOrEmpty(style.Td) ? "" : style.Td;

            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(style_table))
            {
                stringBuilder.Append($"<table style='" + style_table + "'>");
            }
            else
            {
                stringBuilder.Append("<table>");
            }

            // append header
            if (!ignoreHeader)
            {
                stringBuilder.Append("<thead>");
                stringBuilder.Append("<tr>");

                foreach (DataColumn col in dt.Columns)
                {
                    string colName = col.ColumnName;

                    if (columns == null || columns.IsContains(colName))
                    {
                        if (!string.IsNullOrEmpty(style_th))
                        {
                            stringBuilder.Append($"<th style='" + style_th + "'>");
                        }
                        else
                        {
                            stringBuilder.Append("<th>");
                        }
                        stringBuilder.Append(colName);
                        stringBuilder.Append("</th>");
                    }
                }

                stringBuilder.Append("</tr>");
                stringBuilder.Append("</thead>");
            }
            

            // append row
            stringBuilder.Append("<tbody>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                stringBuilder.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (columns == null || columns.IsContains(dt.Columns[j].ColumnName))
                    {
                        if (!string.IsNullOrEmpty(style_td))
                        {
                            stringBuilder.Append($"<td style='" + style_td + "'>");
                        }
                        else
                        {
                            stringBuilder.Append("<td>");
                        }

                        if (dt.Rows[i][j] != DBNull.Value)
                        {
                            string val = dt.Rows[i][j].ToString();
                            if (tdFormatFunc != null)
                            {
                                val = tdFormatFunc(dt, i, j);
                            }
                            // replace string new line to html new line
                            val = val.Replace("\r\n", "<br />");
                            val = val.Replace("\n", "<br />");
                            stringBuilder.Append(val);
                        }
                        stringBuilder.Append("</td>");
                    }
                }
                stringBuilder.Append("</tr>");
            }
            stringBuilder.Append("</tbody>");

            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// convert existing column <paramref name="columnName"/> to new type <paramref name="newType"/>, if existing data type equals to <paramref name="newType"/>, no action wil perform
        /// <para>if convert to System.Data.SqlTypes.SqlBinary, the value will proceed as String value then conver to byte[]</para>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <param name="newType"></param>
        public static void ConvertColumnType(this DataTable dataTable, string columnName, Type newType)
        {
            // if same type, no action required
            if (dataTable.Columns[columnName].DataType.IsEquivalentTo(newType))
            {
                return;
            }

            string tempColumnName = "Dummy_" + Guid.NewGuid().ToString().Replace("-", "");

            using (DataColumn newColumn = new DataColumn(tempColumnName, newType))
            {
                int ordinal = dataTable.Columns[columnName].Ordinal;
                dataTable.Columns.Add(newColumn);
                newColumn.SetOrdinal(ordinal);

                foreach (DataRow row in dataTable.Rows)
                {
                    row[newColumn.ColumnName] = row[columnName] == DBNull.Value ? DBNull.Value : ConvertValueToType(row[columnName], newType);
                }

                dataTable.Columns.Remove(columnName);
                newColumn.ColumnName = columnName;
            }
        }

        /// <summary>
        /// copy <paramref name="dtSource"/> as new DataTable with selected columns from <paramref name="columns"/>
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="columns"/> cannot be null</exception>
        public static DataTable Copy(DataTable dtSource, params string[] columns)
        {
            if (dtSource == null)
            {
                throw new ArgumentNullException(nameof(dtSource), "Source table cannot be null");
            }

            if (columns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(columns), "Column list cannot be null");
            }

            DataTable dtNew = new DataTable();
            foreach (var column in columns)
            {
                DataColumn dtColumn = new DataColumn(column, dtSource.Columns[column].DataType);
                dtNew.Columns.Add(dtColumn);
            }

            foreach (DataRow row in dtSource.Rows)
            {
                DataRow dtRow = dtNew.NewRow();
                foreach (var column in columns)
                {
                    dtRow[column] = row[column];
                }
                dtNew.Rows.Add(dtRow);
            }

            return dtNew;
        }

        /// <summary>
        /// get <paramref name="dtSource"/> as csv format with <paramref name="separator"/>
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="dtSource"/> cannot be null</exception>
        public static string ToString(this DataTable dtSource, string separator)
        {
            if (dtSource == null)
            {
                throw new ArgumentNullException(nameof(dtSource), "Source table cannot be null");
            }

            StringBuilder stringBuilder = new StringBuilder();

            // DataColumn to header
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
                stringBuilder.Append(dtSource.Columns[i].ColumnName);
                if (i < dtSource.Columns.Count - 1)
                {
                    stringBuilder.Append(separator);
                }
            }
            stringBuilder.AppendLine();

            // DataRow to row
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    stringBuilder.Append(dtSource.Rows[i][j].ToString());
                    if (j < dtSource.Columns.Count - 1)
                    {
                        stringBuilder.Append(separator);
                    }
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// merge <paramref name="dt1"/> with <paramref name="dt2"/> based on <paramref name="columnMapping"/>, Key is dt1 column, Value is dt2 column, return new DataTable with Key (dt1) as column name
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        public static DataTable Merge(this DataTable dt1, DataTable dt2, Dictionary<string, string> columnMapping)
        {
            if (dt1 == null)
            {
                throw new ArgumentNullException(nameof(dt1), "Source table cannot be null");
            }
            if (dt2 == null)
            {
                throw new ArgumentNullException(nameof(dt2), "Merge with table cannot be null");
            }

            if (columnMapping == null || columnMapping.Count == 0)
            {
                throw new ArgumentNullException(nameof(columnMapping), "Columns mapping cannot be null or empty");
            }

            // Create a new DataTable to hold the combined data
            DataTable combinedDt = new DataTable();

            // Get the column names from the mapping dictionary
            List<string> columnNames = columnMapping.Keys.Distinct().ToList();

            // Add columns to the combined DataTable
            foreach (string columnName in columnNames)
            {
                combinedDt.Columns.Add(columnName);
            }

            // Combine the rows from both DataTables
            var query = dt1.AsEnumerable()
                .Select(row => columnMapping.ToDictionary(kvp => kvp.Key, kvp => row.Field<object>(kvp.Key)))
                .Concat(dt2.AsEnumerable().Select(row => columnMapping.ToDictionary(kvp => kvp.Value, kvp => row.Field<object>(kvp.Value))));

            // Add the combined rows to the new DataTable
            foreach (var rowValues in query)
            {
                combinedDt.Rows.Add(rowValues.Values.ToArray());
            }

            return combinedDt;
        }

        /// <summary>
        /// distinct DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable Distinct(this DataTable dt)
        {
            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt), "Source table cannot be null");
            }

            // Create a new DataTable to hold the distinct rows
            DataTable distinctDt = dt.Clone();

            // Convert the DataTable to a list of DataRow arrays
            var rows = dt.AsEnumerable().Distinct(DataRowComparer.Default);

            // Add the distinct rows to the new DataTable
            foreach (DataRow row in rows)
            {
                distinctDt.Rows.Add(row.ItemArray);
            }

            return distinctDt;
        }


        /// <summary>
        /// group by <paramref name="dt"/> with <paramref name="columnNames"/>, return new DataTable with Count column to indicate each group by key has how many records
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static DataTable GroupBy(this DataTable dt, params string[] columnNames)
        {
            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt), "Source table cannot be null");
            }

            if (columnNames.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(columnNames), "Columns to groupBy cannot be null");
            }

            // Create a new DataTable to hold the duplicate rows and count
            DataTable duplicateDt = new DataTable();

            // Add columns to the new DataTable
            foreach (string columnName in columnNames)
            {
                duplicateDt.Columns.Add(columnName);
            }
            duplicateDt.Columns.Add("Count", typeof(int));

            var groups = dt.AsEnumerable()
                .GroupBy(row => columnNames.Select(c => row.Field<object>(c)), new AnonymousObjectComparer())
                //.Where(g => g.Count() > 1)
                .Select(g =>
                {
                    var key = g.Key;
                    var count = g.Count();

                    var newRow = duplicateDt.NewRow();
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        newRow[columnNames[i]] = g.First()[columnNames[i]];
                    }
                    newRow["Count"] = count;

                    return newRow;
                });

            // Add the duplicate rows and count to the new DataTable
            foreach (var row in groups)
            {
                duplicateDt.Rows.Add(row);
            }

            return duplicateDt;
        }

        private class AnonymousObjectComparer : IEqualityComparer<IEnumerable<object>>
        {
            public bool Equals(IEnumerable<object> x, IEnumerable<object> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<object> obj)
            {
                return obj.Aggregate(0, (hash, value) => hash ^ value.GetHashCode());
            }
        }

        /// <summary>
        /// from this table or <paramref name="dt1"/> exclude rows present in <paramref name="dt2"/> based on <paramref name="keysMap"/>
        /// <para>keysMap.Key: column name from <paramref name="dt1"/></para>
        /// <para>keysMap.Value: column name from <paramref name="dt2"/></para>
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="keysMap"></param>
        /// <returns></returns>
        public static DataTable ExcludeRows(this DataTable dt1, DataTable dt2, Dictionary<string, string> keysMap)
        {
            if (dt1 == null)
            {
                throw new ArgumentNullException(nameof(dt1), "Table1 cannot be null");
            }
            if (dt2 == null)
            {
                throw new ArgumentNullException(nameof(dt2), "Table2 cannot be null");
            }

            if (keysMap == null || keysMap.Count == 0)
            {
                throw new ArgumentNullException(nameof(keysMap), "keys map cannot be empty");
            }

            DataTable dtResult = dt1.Clone();

            // Create a dictionary to store rows from dt2, use HashSet for better performance
            var dt2RowsKeyHash = dt2.AsEnumerable().Select(row => GetCompositeKey(row, keysMap));
            HashSet<string> dt2Hash = new HashSet<string>(dt2RowsKeyHash);

            // Get the rows from dt1 that are not present in dt2
            var rowsToInclude = dt1.AsEnumerable()
                                   .Where(row1 => !dt2Hash.Contains(GetCompositeKey(row1, keysMap)));

            if (rowsToInclude.Count() > 0)
            {
                dtResult.Merge(rowsToInclude.CopyToDataTable());                
            }

            return dtResult;
        }
        private static string GetCompositeKey(DataRow row, Dictionary<string, string> keysMap)
        {
            return string.Join("_", keysMap.Select(kvp => row.Field<object>(kvp.Key)?.ToString().GetHashCode()));
        }
    }
}

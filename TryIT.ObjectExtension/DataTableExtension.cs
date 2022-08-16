using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                                if (prop.PropertyType.IsGenericType && prop.PropertyType.Name.Contains("Nullable"))
                                {
                                    if (!string.IsNullOrEmpty(row[colName].ToString()))
                                    {
                                        prop.SetValue(obj, ConvertValueToType(row[colName], Nullable.GetUnderlyingType(prop.PropertyType)));
                                    }
                                }
                                else
                                {
                                    prop.SetValue(obj, ConvertValueToType(row[colName], prop.PropertyType), null);
                                }
                            }
                        }
                    }
                    list.Add(obj);
                }
            }
            return list;
        }
        private static object ConvertValueToType(object value, Type type)
        {
            try
            {
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
        /// <returns></returns>
        public static string ToHtmlString(this DataTable dt, ToHtmlString_TableStyle style = null, Delegate_TdFormat tdFormatFunc = null, IEnumerable<string> columns = null)
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
                    Th = "border: 1px solid;",
                    Td = "border: 1px solid;"
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
    }
}

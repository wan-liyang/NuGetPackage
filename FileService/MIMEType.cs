using System;
using System.Collections.Generic;
using System.Text;

namespace FileService
{
    public class MIMEType
    {
        /// <summary>
        /// Any kind of binary data
        /// </summary>
        public static readonly string BIN = "application/octet-stream";
        /// <summary>
        /// Microsoft Excel
        /// </summary>
        public static readonly string XLS = "application/vnd.ms-excel";
        /// <summary>
        /// Microsoft Excel (OpenXML)
        /// </summary>
        public static readonly string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Comma-separated values (CSV)
        /// </summary>
        public static readonly string CSV = "text/csv";


        /// <summary>
        /// get content type by fileName.
        /// <para>default application/octet-stream</para>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentType(string fileName)
        {
            string type = string.Empty;
            string extension = System.IO.Path.GetExtension(fileName);
            switch (extension.ToUpper())
            {
                case ".XLS":
                    type = XLS;
                    break;
                case ".XLSX":
                    type = XLSX;
                    break;
                case ".CSV":
                    type = CSV;
                    break;
                default:
                    type = BIN;
                    break;
            }
            return type;
        }
    }
}

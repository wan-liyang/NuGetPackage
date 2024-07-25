using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace TryIT.Csv
{
    /// <summary>
    /// initial Csv helper
    /// </summary>
    public class Csv
    {
        /// <summary>
        /// get csv file as table
        /// </summary>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        public static DataTable ReadAsDataTable(CsvReaderConfig csvConfig)
        {
            if (!csvConfig.HasHeaderRecord && (csvConfig.Header == null || csvConfig.Header.Length == 0))
            {
                throw new ArgumentNullException(nameof(csvConfig.Header), $"{nameof(csvConfig.Header)} is required when {nameof(csvConfig.HasHeaderRecord)} is false");
            }

            string filePath = csvConfig.FilePath;

            // if file has no header, to create a tmp file with header row, then CsvReader able to read row correctly
            if (!csvConfig.HasHeaderRecord)
            {
                filePath = AddHeader(filePath, csvConfig.Header, csvConfig.Delimiter);
            }

            List<string> badRecord = new List<string>();
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = csvConfig.Delimiter,
                IgnoreReferences = true,
                BadDataFound = (readContext) => badRecord.Add(readContext.RawRecord),
                MissingFieldFound = null
            };

            if (csvConfig.CsvHelperCsvMode == "NoEscape")
            {
                config.Mode = CsvHelper.CsvMode.NoEscape;
            }
            else if (csvConfig.CsvHelperCsvMode == "Escape")
            {
                config.Mode = CsvHelper.CsvMode.Escape;
            }

            var dt = new DataTable();
            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvHelper.CsvReader(reader, config))
                {
                    using (var dr = new CsvHelper.CsvDataReader(csv))
                    {
                        dt.Load(dr);
                    }
                }
            };

            // delete the tmp file after read
            if (!csvConfig.HasHeaderRecord)
            {
                File.Delete(filePath);
            }

            // delete skip header or footer row from table
            {
                int skipHeader = csvConfig.SkipHeaderRows;
                while (skipHeader > 0)
                {
                    dt.Rows[0].Delete();
                    dt.AcceptChanges();
                    skipHeader--;
                }

                int skipFooter = csvConfig.SkipFooterRows;
                while (skipFooter > 0)
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                    dt.AcceptChanges();
                    skipFooter--;
                }
            }

            return dt;
        }

        /// <summary>
        /// add header row into file
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <param name="header"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string AddHeader(string fileNameAndPath, string[] header, string delimiter)
        {
            string tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            {
                using (var reader = new StreamReader(fileNameAndPath))
                {
                    writer.WriteLine(string.Join(delimiter, header));
                    while (!reader.EndOfStream)
                    {
                        writer.WriteLine(reader.ReadLine());
                    }
                }
            }

            return tempfile;
        }

        /// <summary>
		/// save DataTable as Csv file, overwrite if exists
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="csvConfig"></param>
        public static void SaveAs(DataTable dataTable, CsvWriterConfig csvConfig)
        {
            var dir = Path.GetDirectoryName(csvConfig.OutputFileNameAndPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = csvConfig.Delimiter
            };

            if (csvConfig.AlwaysQuote)
            {
                config.ShouldQuote = args => true;
            }

            bool hasPreviousRow = false;

            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, config))
            {
                if (!string.IsNullOrEmpty(csvConfig.FirstLineValue))
                {
                    csv.WriteField(csvConfig.FirstLineValue, false);
                    hasPreviousRow = true;
                }

                if (!csvConfig.SkipHeader)
                {
                    NextRecord(csv, hasPreviousRow);
                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        csv.WriteField(dc.ColumnName);
                    }
                    hasPreviousRow = true;
                }                

                foreach (DataRow dr in dataTable.Rows)
                {
                    NextRecord(csv, hasPreviousRow);
                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        csv.WriteField(dr[dc]);
                    }
                    hasPreviousRow = true;
                }

                if (!string.IsNullOrEmpty(csvConfig.LastLineValue))
                {
                    NextRecord(csv, hasPreviousRow);
                    csv.WriteField(csvConfig.LastLineValue);
                }

                csv.Flush();
                File.WriteAllText(csvConfig.OutputFileNameAndPath, writer.ToString());
            }
        }

        /// <summary>
        /// end current row and start a new row, when the <paramref name="hasPreviousRow"/> is true
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="hasPreviousRow"></param>
        private static void NextRecord(CsvWriter csv, bool hasPreviousRow)
        {
            if (hasPreviousRow)
            {
                csv.NextRecord();
            }
        }
    }
}

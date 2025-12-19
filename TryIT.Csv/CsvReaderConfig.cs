using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.Csv
{
    /// <summary>
    /// configuration for read csv file
    /// </summary>
    public class CsvReaderConfig
    {
        /// <summary>
        /// delimiter to get data as table
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// indicate whether source file has header record
        /// </summary>
        public bool HasHeaderRecord { get; set; }

        /// <summary>
        /// if <see cref="HasHeaderRecord"/> is false, need provide this array to populate data as table
        /// </summary>
        public string[] Header { get; set; }

        /// <summary>
        /// NoEscape / Escape / None (default)
        /// </summary>
        public string CsvHelperCsvMode { get; set; }

        /// <summary>
        /// number of header row to skip, it will remove from DataTable result
        /// </summary>
        public int SkipHeaderRows { get; set; }

        /// <summary>
        /// number of footer row to skip, it will remove from DataTable result
        /// </summary>
        public int SkipFooterRows { get; set; }
    }
}

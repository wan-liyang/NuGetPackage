using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.Csv
{
    /// <summary>
    /// configuration for write into csv file
    /// </summary>
    public class CsvWriterConfig
    {
        /// <summary>
        /// file name and path where csv save to
        /// </summary>
        public string OutputFileNameAndPath { get; set; }

        /// <summary>
        /// csv delimiter
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// indicator always put content inside quote
        /// </summary>
        public bool AlwaysQuote { get; set; }
    }
}

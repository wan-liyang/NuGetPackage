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

        /// <summary>
        /// value to put in first line
        /// </summary>
        public string FirstLineValue { get; set; }

        /// <summary>
        /// value to put in last line
        /// </summary>
        public string LastLineValue { get; set; }

        /// <summary>
        /// indicate whether skip write header
        /// </summary>
        public bool SkipHeader { get; set; }
    }
}

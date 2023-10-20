using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.CopyMode
{
    /// <summary>
    /// update and insert
    /// </summary>
    public class CopyMode_InsertUpdate : CopyModeBase
    {
        /// <summary>
        /// (mandatory) primary key on target table
        /// </summary>
        public List<string> PrimaryKeys { get; set; }

        /// <summary>
        /// (optional) timestamp column to indicate when the record be updated, for insert, the timestamp should have default contraint to give default value
        /// </summary>
        public string TimestampColumn { get; set; }
    }
}

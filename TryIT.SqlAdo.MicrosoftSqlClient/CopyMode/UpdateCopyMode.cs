using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.CopyMode
{
    /// <summary>
    /// update and insert
    /// </summary>
    public class UpdateCopyMode : CopyModeBase
    {
        /// <summary>
        /// (mandatory) primary key column on target table, use to perform update and insert
        /// </summary>
        public List<string> PrimaryKeys { get; set; }

        /// <summary>
        /// (optional) timestamp column to indicate when the record be updated, for insert, the timestamp should have default contraint to give default value
        /// </summary>
        public string TimestampColumn { get; set; }
    }
}

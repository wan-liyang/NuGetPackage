﻿using System.Collections.Generic;
using System.Data;

namespace TryIT.SqlAdo.MicrosoftSqlClient.CopyMode
{
    /// <summary>
    /// common property for copy mode
    /// </summary>
    public abstract class CopyModeBase : ICopyMode
    {
        /// <summary>
        /// (mandatory) source data table to copy
        /// </summary>
        public DataTable SourceData { get; set; }

        /// <summary>
        /// (mandatory) target table copy data into, it must contains schema, e.g. schema.table
        /// </summary>
        public string TargetTable { get; set; }

        /// <summary>
        /// (mandatory) column mappings between source data table and target table (source.column => target.column)
        /// <para>case-insensitive</para>
        /// <para>if null or empty, will use source data table as mapping (source.column => source.column)</para>
        /// </summary>
        public Dictionary<string, string> ColumnMappings { get; set; }

        /// <summary>
        /// (optional) pre execute script before copy data, within the transaction, it will be execute by ExecuteNonQuery
        /// <para>it is mandatory for <see cref="CopyMode_DeleteInsert"/> use to perform delete action before insert data</para>
        /// </summary>
        public string PreScript { get; set; }

        /// <summary>
        /// (optional) post execute script after copy data, within the transaction, it will be execute by ExecuteNonQuery
        /// </summary>
        public string PostScript { get; set; }
    }
}

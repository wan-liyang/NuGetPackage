using System.Collections.Generic;
using System.Data;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// define copy data information
    /// </summary>
    public class CopyDataModel
    {
        /// <summary>
        /// source data table to copy
        /// </summary>
        public DataTable SourceData { get; set; }
        /// <summary>
        /// target table copy data into, it must contains schema, e.g. schema.table
        /// </summary>
        public string TargetTable { get; set; }
        /// <summary>
        /// column mappings between source data table and target table, case-sensitive on source column and destination column, if this property is null or empty, will assume source data table and target table has same ColumnName and OrdinalPosition
        /// </summary>
        public Dictionary<string, string> ColumnMappings { get; set; }

        /// <summary>
        /// indicator copy mode
        /// </summary>
        public CopyMode CopyMode { get; set; }

        /// <summary>
        /// sql condition to delete record from target table, e.g. WHERE Column1 &lt;= 1, this is mandatory when <see cref="CopyDataModel.CopyMode"/> is <see cref="CopyMode.DELETE_INSERT"/>
        /// </summary>
        public string DeleteCondition { get; set; }

        /// <summary>
        /// pre execute script before copy data, within the transaction, it will be execute by ExecuteNonQuery
        /// </summary>
        public string PreScript { get; set; }

        /// <summary>
        /// post execute script after copy data, within the transaction, it will be execute by ExecuteNonQuery
        /// </summary>
        public string PostScript { get; set; }
    }

    /// <summary>
    /// copy mode
    /// </summary>
    public enum CopyMode
    {
        /// <summary>
        /// truncate data before copy, then insert record from source data table into target table
        /// </summary>
        TRUNCATE_INSERT,

        /// <summary>
        /// straghtly insert record from source data table into target table
        /// </summary>
        INSERT,

        /// <summary>
        /// delete target record based on <see cref="CopyDataModel.DeleteCondition"/>, then insert record from source data table into target table 
        /// </summary>
        DELETE_INSERT
    }
}

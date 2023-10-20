using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.CopyMode
{
    /// <summary>
    /// do delete and insert
    /// </summary>
    public class CopyMode_DeleteInsert : CopyModeBase
    {
        /// <summary>
        /// sql condition to delete record from target table, e.g. WHERE Column1 &lt;= 1
        /// </summary>
        public string DeleteCondition { get; set; }
    }
}

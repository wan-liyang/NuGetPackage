using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// Delegate for logging HTTP request and response details.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task DbLogDelegate(DbLogContext context);


    /// <summary>
    /// Represents the stage of the HTTP request lifecycle for logging purposes.
    /// </summary>
    public enum LogStage
    {
        /// <summary>
        /// Occurs before the execution.
        /// </summary>
        BeforeExecute,

        /// <summary>
        /// Occurs after the execution.
        /// </summary>
        AfterExecute,

        /// <summary>
        /// Occurs when an exception is thrown during the execution.
        /// </summary>
        OnError
    }

    /// <summary>
    /// Database log context for logging command execution details.
    /// </summary>
    public class DbLogContext
    {
        /// <summary>
        /// trace ID
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// CorrelationId from the context
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Extra correlation properties from the context
        /// </summary>
        public Dictionary<string, string> CorrelationExtra { get; set; }

        /// <summary>
        /// Stage of execution (BeforeExecute, AfterExecute, Error)
        /// </summary>
        public LogStage Stage { get; set; }

        /// <summary>
        /// Database provider (e.g. SqlServer, PostgreSQL)
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Database name or logical identifier
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Data source / server (sanitized)
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// SQL command text (IMPORTANT: should be sanitized)
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Command type (Text, StoredProcedure)
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Parameters (name + value, masked if sensitive)
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Number of rows affected (for non-query)
        /// </summary>
        public int? RowsAffected { get; set; }

        /// <summary>
        /// Result metadata (optional: row count for query)
        /// </summary>
        public int? ResultRowCount { get; set; }

        /// <summary>
        /// Execution duration in milliseconds
        /// </summary>
        public long DurationMs { get; set; }

        /// <summary>
        /// Whether executed inside a transaction
        /// </summary>
        public bool InTransaction { get; set; }

        /// <summary>
        /// Transaction ID (if available)
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Exception (if any)
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Start time UTC
        /// </summary>
        public DateTimeOffset StartTimeUtc { get; set; }

        /// <summary>
        /// End time UTC
        /// </summary>
        public DateTimeOffset EndTimeUtc { get; set; }
    }
}

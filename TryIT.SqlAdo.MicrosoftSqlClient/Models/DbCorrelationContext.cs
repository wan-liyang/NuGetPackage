using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// Represents a correlation context for database operations, containing a correlation ID and extra data.
    /// </summary>
    public class DbCorrelationContext
    {
        /// <summary>
        /// Gets or sets the correlation identifier for the current context.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets additional key-value data for the correlation context.
        /// </summary>
        public Dictionary<string, string> CorrelationExtra { get; set; }
    }

    /// <summary>
    /// Provides access to the current <see cref="DbCorrelationContext"/> for the async flow.
    /// </summary>
    public static class CorrelationContextAccessor
    {
        private static readonly AsyncLocal<DbCorrelationContext> _current = new AsyncLocal<DbCorrelationContext>();

        /// <summary>
        /// Gets or sets the current <see cref="DbCorrelationContext"/> for the async flow.
        /// </summary>
        public static DbCorrelationContext Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }

    /// <summary>
    /// Represents a scope for setting and restoring a <see cref="DbCorrelationContext"/>.
    /// </summary>
    public sealed class CorrelationScope : IDisposable
    {
        private readonly DbCorrelationContext _previous;


        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationScope"/> class and sets the current correlation context.
        /// </summary>
        /// <param name="context">The <see cref="DbCorrelationContext"/> to set as current.</param>
        public CorrelationScope(DbCorrelationContext context)
        {
            _previous = CorrelationContextAccessor.Current;
            CorrelationContextAccessor.Current = context;
        }

        /// <summary>
        /// Restores the previous correlation context when disposed.
        /// </summary>
        public void Dispose()
        {
            CorrelationContextAccessor.Current = _previous;
        }
    }
}

using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryIT.PostgreSql.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectorConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public required string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TokenCredential? TokenCredential { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RetryProperty? RetryProperty { get; set; }

        /// <summary>
        /// log delegate for database operations, it will be called before and after command execution, and when error happens
        /// </summary>
        public DbLogDelegate? DbLogDelegate { get; set; }
    }
}

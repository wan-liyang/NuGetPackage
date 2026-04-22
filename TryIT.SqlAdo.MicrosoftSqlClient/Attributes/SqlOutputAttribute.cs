using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Attributes
{
    /// <summary>  
    /// Get or set if the property is updatable in SQL  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SqlOutputAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOutputAttribute"/> class, specifying whether the
        /// associated SQL entity is returned.
        /// </summary>
        public SqlOutputAttribute()
        {
        }
    }
}

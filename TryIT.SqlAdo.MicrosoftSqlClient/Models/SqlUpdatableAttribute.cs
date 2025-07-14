using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>  
    /// Get or set if the property is updatable in SQL  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SqlUpdatableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUpdatableAttribute"/> class, specifying whether the
        /// associated SQL entity is updatable.
        /// </summary>
        /// <param name="_isUpdatable">A value indicating whether the SQL entity can be updated.  <see langword="true"/> if the entity is
        /// updatable; otherwise, <see langword="false"/>.</param>
        public SqlUpdatableAttribute(bool _isUpdatable)
        {
            IsUpdatable = _isUpdatable;
        }

        /// <summary>
        /// Gets a value indicating whether the current object can be updated.
        /// </summary>
        public bool IsUpdatable { get; }
    }
}

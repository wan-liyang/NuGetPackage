using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.ActiveDirectory.Entities
{
    /// <summary>
    /// Group member
    /// </summary>
    public class AdGroupMember
    {
        /// <summary>
        /// User information
        /// </summary>
        public AdUser AdUser { get; set; }

        /// <summary>
        /// Group from which the user is inherited, value is empty if the user is direct member of the group
        /// </summary>
        public string InheritedFromGroup { get; set; }
    }
}

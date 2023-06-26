using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.Model
{
    /// <summary>
    /// group member information
    /// </summary>
    public class GroupMemberModel
    {
        /// <summary>
        /// member object id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// mail
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// principal name
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.Model
{
    /// <summary>
    /// group information
    /// </summary>
    public class GroupModel
    {
        /// <summary>
        /// group object id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// group display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// group description
        /// </summary>
        public string Description { get; set; }
    }
}

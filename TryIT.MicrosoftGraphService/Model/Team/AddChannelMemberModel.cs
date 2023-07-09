using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.Model.Team
{
    /// <summary>
    /// add channel member
    /// </summary>
    public class AddChannelMemberModel
    {
        /// <summary>
         /// team name
         /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// channel name
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// user email address
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// member role
        /// </summary>
        public MemberRole Role { get; set; }
    }
}

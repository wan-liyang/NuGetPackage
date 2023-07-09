using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.Model.Team
{
    /// <summary>
    /// add member to a team
    /// </summary>
    public class AddMemberModel
    {
        /// <summary>
        /// team id, if this is not null, will ignore <see cref="TeamName"/>
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// team name
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// user email address
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// member role
        /// </summary>
        public MemberRole Role { get; set; }
    }

    public enum MemberRole
    {
        member,
        owner
    }
}

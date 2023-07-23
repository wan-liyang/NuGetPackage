using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Team
{
    /// <summary>
    /// add member to a team
    /// </summary>
    public class AddMemberModel
    {
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
        public MemberRoleEnum Role { get; set; }
    }
}

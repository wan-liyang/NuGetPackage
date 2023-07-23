using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Team
{
    /// <summary>
    /// detail for create team
    /// </summary>
    public class CreateTeamModel
    {
        /// <summary>
        /// team display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// team description
        /// </summary>
        public string Description { get; set; }
    }
}

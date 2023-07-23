using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Team
{
    /// <summary>
    /// create channel
    /// </summary>
    public class CreateChannelModel
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
        /// channel description
        /// </summary>
        public string Description { get; set; }
    }
}

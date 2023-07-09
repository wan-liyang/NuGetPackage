using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class CreateChannelRequest
    {
        public string displayName { get; set; }
        public string description { get; set; }
        public string membershipType { get; set; }

    }
}

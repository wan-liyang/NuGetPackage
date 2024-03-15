using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.DataFactory
{
    public class BaseRequest
    {
        public string subscriptionId { get; set; }
        public string resourceGroupName { get; set; }
        public string factoryName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.DataFactory
{
    public class GetPipelineRunRequest : BaseRequest
    {
        public string runId { get; set; }
    }
}

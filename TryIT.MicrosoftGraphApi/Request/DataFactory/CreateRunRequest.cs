using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.DataFactory
{
    public class CreateRunRequest : BaseRequest
    {
        public string pipelineName { get; set; }
        /// <summary>
        /// parameters in json format
        /// </summary>
        public string ParametersJson { get; set; }
    }
}

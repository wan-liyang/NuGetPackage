using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Databricks
{
    public class GetJobRunRequest : BaseRequest
    {
        public long run_id { get; set; }
    }
}

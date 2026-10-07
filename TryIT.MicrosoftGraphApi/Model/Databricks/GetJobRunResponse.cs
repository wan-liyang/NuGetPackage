using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Databricks
{
    public class GetJobRunResponse
    {
        public long run_id { get; set; }
        public state state { get; set; }
    }

    public class state
    {
        public string life_cycle_state { get; set; }

        /// <summary>
        /// Gets or sets the result state of the operation. SUCCESS, FAILED, or CANCELED. This field is only available when the life_cycle_state is TERMINATED.
        /// </summary>
        public string result_state { get; set; }
        public string state_message { get; set; }
    }
}

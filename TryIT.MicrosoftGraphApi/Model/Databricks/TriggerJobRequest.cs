using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Databricks
{
    /// <summary>
    /// Represents a request to trigger a job in a specified workspace.
    /// </summary>
    public class TriggerJobRequest : BaseRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the job.
        /// </summary>
        public string job_id { get; set; }
    }
}

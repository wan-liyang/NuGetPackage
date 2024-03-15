using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.DataFactory
{
    public class GetPipelineRunsResponse
    {
        public class Parameters
        {
        }

        public class InvokedBy
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string invokedByType { get; set; }
        }

        public class PipelineReturnValue
        {
        }

        public class RunDimension
        {
        }

        public class Response
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string runId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string debugRunId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string runGroupId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pipelineName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Parameters parameters { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public InvokedBy invokedBy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string runStart { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string runEnd { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int? durationInMs { get; set; }
            /// <summary>
            /// The status of a pipeline run.Possible values: Queued, InProgress, Succeeded, Failed, Canceling, Cancelled
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public PipelineReturnValue pipelineReturnValue { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string lastUpdated { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> annotations { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public RunDimension runDimension { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isLatest { get; set; }
        }

    }
}

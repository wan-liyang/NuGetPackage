using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Databricks;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class DatabricksHelper : BaseHelper
    {
        public DatabricksHelper(MsGraphApiConfig config) : base(config) { }

        public async Task<TriggerJobResponse> TriggerJobAsync(TriggerJobRequest triggerJobRequest)
        {
            string url = $"{triggerJobRequest.WorkspaceUrl}/api/2.1/jobs/run-now";
            var body = new
            {
                job_id = triggerJobRequest.job_id
            };

            var response = await RestApi.PostAsync(url, GetJsonHttpContent(body));
            CheckStatusCode(response);

            string result = await response.Content.ReadAsStringAsync();
            return result.JsonToObject<TriggerJobResponse>();
        }

        public async Task<GetJobRunResponse> GetJobRunAsync(GetJobRunRequest getJobRunRequest)
        {
            string url = $"{getJobRunRequest.WorkspaceUrl}/api/2.1/jobs/runs/get?run_id={getJobRunRequest.run_id}";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string result = await response.Content.ReadAsStringAsync();
            return result.JsonToObject<GetJobRunResponse>();
        }
    }
}

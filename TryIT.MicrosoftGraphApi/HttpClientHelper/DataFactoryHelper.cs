using System;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.DataFactory;
using TryIT.MicrosoftGraphApi.Response.DataFactory;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class DataFactoryHelper : BaseHelper
    {
        public DataFactoryHelper(MsGraphApiConfig config) : base(config) { }

        /// <summary>
        /// Creates a run of a pipeline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateRunResponse.Response CreateRun(CreateRunRequest request)
        {
            string url = $"https://management.azure.com/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroupName}/providers/Microsoft.DataFactory/factories/{request.factoryName}/pipelines/{request.pipelineName}/createRun?api-version=2018-06-01";

            HttpContent httpContent = new StringContent("");
            if (!string.IsNullOrEmpty(request.ParametersJson))
            {
                httpContent = new StringContent(request.ParametersJson);
            }

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<CreateRunResponse.Response>();
        }

        /// <summary>
        /// Get a pipeline run by its run ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetPipelineRunsResponse.Response GetPipelineRun(GetPipelineRunRequest request)
        {
            string url = $"https://management.azure.com/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroupName}/providers/Microsoft.DataFactory/factories/{request.factoryName}/pipelineruns/{request.runId}?api-version=2018-06-01";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetPipelineRunsResponse.Response>();
        }
    }
}

using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<CreateRunResponse.Response> CreateRunAsync(CreateRunRequest request)
        {
            string url = $"https://management.azure.com/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroupName}/providers/Microsoft.DataFactory/factories/{request.factoryName}/pipelines/{request.pipelineName}/createRun?api-version=2018-06-01";

            HttpContent httpContent = new StringContent("");
            if (!string.IsNullOrEmpty(request.ParametersJson))
            {
                httpContent = new StringContent(request.ParametersJson);
            }

            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<CreateRunResponse.Response>();
        }

        /// <summary>
        /// Get a pipeline run by its run ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPipelineRunsResponse.Response> GetPipelineRunAsync(GetPipelineRunRequest request)
        {
            string url = $"https://management.azure.com/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroupName}/providers/Microsoft.DataFactory/factories/{request.factoryName}/pipelineruns/{request.runId}?api-version=2018-06-01";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetPipelineRunsResponse.Response>();
        }
    }
}

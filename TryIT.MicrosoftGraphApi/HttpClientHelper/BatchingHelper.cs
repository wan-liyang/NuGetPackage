using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.Batching;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class BatchingHelper : BaseHelper
    {
        public BatchingHelper(MsGraphApiConfig config) : base(config) { }

        public async Task<List<string>> Post(BatchingRequest.Body requestBody)
        {
            List<string> results = new List<string>();
            BatchingRequest.Body body = new BatchingRequest.Body();

            // max 20 requests per batch https://learn.microsoft.com/en-us/graph/json-batching?utm_source=chatgpt.com&tabs=http,
            // set limit to 10 to avoid issue

            int batchSizeLimit = 10;
            int batchCount = 0;
            for (int i = 0; i < requestBody.requests.Count; i++)
            {
                body.requests.Add(requestBody.requests[i]);
                batchCount++;
                if (batchCount == batchSizeLimit 
                    || (batchCount < batchSizeLimit && i == requestBody.requests.Count - 1))
                {
                    string result = await _postBatch(body);
                    results.Add(result);
                    batchCount = 0;
                    body.requests.Clear();
                }
            }

            return results;
        }

        private async Task<string> _postBatch(BatchingRequest.Body requestBody)
        {
            string url = $"{GraphApiRootUrl}/$batch";
            HttpContent httpContent = GetJsonHttpContent(requestBody);
            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}

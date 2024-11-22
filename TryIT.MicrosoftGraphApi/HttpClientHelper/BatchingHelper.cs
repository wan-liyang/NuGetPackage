using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using TryIT.MicrosoftGraphApi.Request.Batching;
using TryIT.MicrosoftGraphApi.Request.Sharepoint;
using TryIT.MicrosoftGraphApi.Response.DataFactory;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class BatchingHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;
        private readonly HttpClient _httpClient;

        public BatchingHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });
            _httpClient = httpClient;
        }

        public List<string> Post(BatchingRequest.Body requestBody)
        {
            List<string> results = new List<string>();
            BatchingRequest.Body body = new BatchingRequest.Body();

            int batchCount = 0;
            for (int i = 0; i < requestBody.requests.Count; i++)
            {
                body.requests.Add(requestBody.requests[i]);
                batchCount++;
                if (batchCount == 20)
                {
                    string result = _postBatch(body);
                    results.Add(result);
                    batchCount = 0;
                    body.requests.Clear();
                }
                else if (batchCount < 20 && i == requestBody.requests.Count - 1)
                {
                    string result = _postBatch(body);
                    results.Add(result);
                    batchCount = 0;
                    body.requests.Clear();
                }
            }

            return results;
        }

        private string _postBatch(BatchingRequest.Body requestBody)
        {
            string url = $"{GraphApiRootUrl}/$batch";
            try
            {
                HttpContent httpContent = this.GetJsonHttpContent(requestBody);
                var response = api.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content;
            }
            catch
            {
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.Batching;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchApi : BaseHelper
    {
        private UserHelper _helper;
        private HttpClient _httpClient;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public BatchApi(MsGraphApiConfig config)
        {
            _httpClient = new MsGraphHelper(config).GetHttpClient();
            _helper = new UserHelper(_httpClient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<List<string>> Post(BatchingRequest.Body body)
        {
            var batching = new BatchingHelper(_httpClient);
            return await batching.Post(body);
        }
    }
}

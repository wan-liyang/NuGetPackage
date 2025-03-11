using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.Batching;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchApi
    {
        private readonly BatchingHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public BatchApi(MsGraphApiConfig config)
        {
            _helper = new BatchingHelper(config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<List<string>> Post(BatchingRequest.Body body)
        {
            return await _helper.Post(body);
        }
    }
}

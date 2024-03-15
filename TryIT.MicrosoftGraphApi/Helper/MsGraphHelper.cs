using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Model;

namespace TryIT.MicrosoftGraphApi.Helper
{
    internal class MsGraphHelper
    {
        private static HttpClient _httpClient;

        public MsGraphHelper(MsGraphApiConfig config)
        {
            WebProxy proxy = WebProxyHelper.GetProxy(config.Proxy);

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            if (proxy != null)
            {
                clientHandler.Proxy = proxy;
            }

            _httpClient = new HttpClient(clientHandler);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + config.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            if (config.TimeoutSecond > 0)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSecond);
            }
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}

using TryIT.MicrosoftGraphService.Model;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace TryIT.MicrosoftGraphService.Helper
{
    internal class MsGraphHelper
    {
        private static HttpClient _httpClient;

        public MsGraphHelper(MsGraphApiConfig config)
        {
            if (string.IsNullOrEmpty(config.Token))
            {
                throw new System.ArgumentNullException(nameof(config.Token));
            }

            WebProxy proxy = WebProxyHelper.GetProxy(config.Proxy_Url, config.Proxy_Username, config.Proxy_Password);

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

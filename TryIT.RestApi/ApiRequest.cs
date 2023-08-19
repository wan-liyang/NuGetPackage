using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;

namespace TryIT.RestApi
{
    /// <summary>
    /// initial API request
    /// </summary>
    public class ApiRequest
    {
        private static WebProxy GetWebProxy(WebProxyInfo webProxyInfo)
        {
            if (!string.IsNullOrEmpty(webProxyInfo.Url))
            {
                WebProxy proxy = new WebProxy(webProxyInfo.Url);
                proxy.UseDefaultCredentials = true;

                if (!string.IsNullOrEmpty(webProxyInfo.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(webProxyInfo.Username, webProxyInfo.Password);
                }
                proxy.BypassProxyOnLocal = true;

                return proxy;
            }
            return null;
        }

        private static HttpClient GetHttpClient(RequestModel request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrEmpty(request.Url))
            {
                throw new ArgumentNullException(nameof(request.Url));
            }

            string url = request.Url;

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            if (request.WebProxy != null)
            {
                var proxy = GetWebProxy(request.WebProxy);
                if (proxy != null)
                {
                    clientHandler.Proxy = proxy;
                }
            }
            HttpClient client = new HttpClient(clientHandler);

            if (request.TimeoutSecond > 0)
            {
                client.Timeout = TimeSpan.FromSeconds(request.TimeoutSecond);
            }

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (request.BasicAuth != null)
            {
                if (!string.IsNullOrEmpty(request.BasicAuth.Username) || !string.IsNullOrEmpty(request.BasicAuth.Password))
                {
                    string basicToken = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{request.BasicAuth.Username}:{request.BasicAuth.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
                }
            }

            if (request.Headers != null && request.Headers.Count > 0)
            {
                foreach (var item in request.Headers)
                {
                    client.DefaultRequestHeaders.Remove(item.Key);
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            return client;
        }

        /// <summary>
        /// call Rest Api with GET
        /// </summary>
        /// <param name="request"></param>
        /// <param name="securityProtocolType">default use <see cref="SecurityProtocolType.Tls12"/> for more secure</param>
        /// <returns></returns>
        public static ResponseModel Get(RequestModel request, SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12)
        {
            HttpClient client = GetHttpClient(request);

            try
            {
                ServicePointManager.SecurityProtocol = securityProtocolType;
                var clientResult = client.GetAsync(request.Url).GetAwaiter().GetResult();

                return new ResponseModel
                {
                    StatusCode = clientResult.StatusCode,
                    Content = clientResult.Content
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }

        /// <summary>
        /// call Rest Api with POST
        /// </summary>
        /// <param name="request"></param>
        /// <param name="securityProtocolType">default use <see cref="SecurityProtocolType.Tls12"/> for more secure</param>
        /// <returns></returns>
        public static ResponseModel Post(RequestModel request, SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12)
        {
            HttpClient client = GetHttpClient(request);

            try
            {
                ServicePointManager.SecurityProtocol = securityProtocolType;

                if (string.IsNullOrEmpty(request.Body))
                {
                    request.Body = "";
                }
                StringContent requestContent = new StringContent(request.Body, System.Text.Encoding.UTF8, "application/json");

                var clientResult = client.PostAsync(request.Url, requestContent).GetAwaiter().GetResult();

                return new ResponseModel
                {
                    StatusCode = clientResult.StatusCode,
                    Content = clientResult.Content
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}

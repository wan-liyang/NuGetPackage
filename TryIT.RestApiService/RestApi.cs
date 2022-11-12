using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TryIT.RestApiService
{
    public class RestApi
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

        private static HttpClient GetHttpClient(ApiRequest request)
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

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(request.Username) || !string.IsNullOrEmpty(request.Password))
            {
                string basicToken = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{request.Username}:{request.Password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
            }

            return client;
        }

        private static SecurityProtocolType GetSecurityProtocol()
        {
            /*
             * force to use Tls12 protocol

                Insecure Transport: Weak SSL Protocol (4 issues)

                Abstract
                The SSLv2, SSLv23, SSLv3, TLSv1.0 and TLSv1.1 protocols contain flaws that make them insecure and
                should not be used to transmit sensitive data.

                Explanation
                The Transport Layer Security (TLS) and Secure Sockets Layer (SSL) protocols provide a protection
                mechanism to ensure the authenticity, confidentiality, and integrity of data transmitted between a client and
                web server. Both TLS and SSL have undergone revisions resulting in periodic version updates. Each new
                revision is designed to address the security weaknesses discovered in previous versions. Use of an
                insecure version of TLS/SSL weakens the data protection strength and might allow an attacker to
                compromise, steal, or modify sensitive information.
                Weak versions of TLS/SSL might exhibit one or more of the following properties:
                - No protection against man-in-the-middle attacks - Same key used for authentication and encryption -
                Weak message authentication control - No protection against TCP connection closing - Use of weak cipher
                suites
                The presence of these properties might allow an attacker to intercept, modify, or tamper with sensitive data.

                Recommendation
                Fortify highly recommends forcing the client to use only the most secure protocols.                 
            */

            return SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// call Rest Api with GET
        /// </summary>
        /// <param name="request"></param>
        /// <param name="securityProtocolType">default use <see cref="SecurityProtocolType.Tls12"/> for more secure</param>
        /// <returns></returns>
        public static ApiResponse Get(ApiRequest request, SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12)
        {
            HttpClient client = GetHttpClient(request);

            try
            {
                ServicePointManager.SecurityProtocol = securityProtocolType;
                var clientResult = client.GetAsync(request.Url).GetAwaiter().GetResult();

                return new ApiResponse
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
        public static ApiResponse Post(ApiRequest request, SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12)
        {
            HttpClient client = GetHttpClient(request);

            try
            {
                ServicePointManager.SecurityProtocol = securityProtocolType;

                StringContent requestContent = new StringContent(request.Body, System.Text.Encoding.UTF8, "application/json");

                var clientResult = client.PostAsync(request.Url, requestContent).GetAwaiter().GetResult();

                return new ApiResponse
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

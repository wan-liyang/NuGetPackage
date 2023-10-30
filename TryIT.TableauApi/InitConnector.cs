using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.SiteModel;

namespace TryIT.TableauApi
{
    public partial class TableauConnector : IDisposable
    {
        string siteId;
        string myId;
        string apiVersion;

        HttpClient httpClient;

        private static WebProxy GetProxy(WebProxyModel proxyModel)
        {
            WebProxy proxy = null;

            if (proxyModel != null && !string.IsNullOrEmpty(proxyModel.Url))
            {
                proxy = new WebProxy(proxyModel.Url);

                if (!string.IsNullOrEmpty(proxyModel.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(proxyModel.Username, proxyModel.Password);
                }
                proxy.BypassProxyOnLocal = true;
            }

            return proxy;
        }

        private static void ValidateInfo(ApiRequestModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }
            if (string.IsNullOrEmpty(requestModel.HostUrl))
            {
                throw new ArgumentNullException(nameof(requestModel.HostUrl));
            }
            if (string.IsNullOrEmpty(requestModel.Sitename))
            {
                throw new ArgumentNullException(nameof(requestModel.Sitename));
            }
            if (string.IsNullOrEmpty(requestModel.ApiVersion))
            {
                throw new ArgumentNullException(nameof(requestModel.ApiVersion));
            }
            if (string.IsNullOrEmpty(requestModel.TokenName))
            {
                throw new ArgumentNullException(nameof(requestModel.TokenName));
            }
            if (string.IsNullOrEmpty(requestModel.TokenSecret))
            {
                throw new ArgumentNullException(nameof(requestModel.TokenSecret));
            }
        }

        /// <summary>
        /// init Tableau Connector
        /// </summary>
        /// <param name="requestModel"></param>
        public TableauConnector(ApiRequestModel requestModel)
        {
            ValidateInfo(requestModel);

            this.apiVersion = requestModel.ApiVersion;

            WebProxy proxy = GetProxy(requestModel.Proxy);
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            if (proxy != null)
            {
                clientHandler.Proxy = proxy;
            }

            httpClient = new HttpClient(clientHandler);
            httpClient.BaseAddress = new Uri(requestModel.HostUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"/api/{apiVersion}/auth/signin";
            string request = $"<tsRequest><credentials personalAccessTokenName=\"{requestModel.TokenName}\" personalAccessTokenSecret=\"{requestModel.TokenSecret}\"><site contentUrl=\"{requestModel.Sitename}\"/></credentials></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var signinResponse = content.JsonToObject<SigninResponse.Response>();
            this.siteId = signinResponse.credentials.site.id;
            this.myId = signinResponse.credentials.user.id;

            // add the token into httpClient
            string token = signinResponse.credentials.token;
            httpClient.DefaultRequestHeaders.Remove("X-Tableau-Auth");
            httpClient.DefaultRequestHeaders.Add("X-Tableau-Auth", token);
        }

        public void Dispose()
        {
            siteId = null;
            myId = null;
            httpClient = null;
        }

        /// <summary>
        /// check API response status, if failed, throw exception
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <exception cref="Exception"></exception>
        private void CheckResponseStatus(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"operation failed: {responseMessage.Content.ReadAsStringAsync().Result}");
            }
        }

        /// <summary>
        /// get total pages based on PageSize and TotalAvailable items
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalAvailable"></param>
        private int GetTotalPages(int pageSize, int totalAvailable)
        {
            int pages = totalAvailable / pageSize;

            if (totalAvailable % pageSize > 0)
            {
                pages++;
            }

            return pages;
        }
    }
}

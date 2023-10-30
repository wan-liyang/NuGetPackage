using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.ApiResponse.Datasource;

namespace TryIT.TableauApi
{
    public partial class TableauConnector
    {
        /// <summary>
        /// get specific datasource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GetDataSourceResponse.Datasource GetDatasource(string name)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/datasources?filter=name:eq:{name}";

            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<GetDataSourceResponse.TsResponse>();
            if (result.Datasources.Datasource == null)
            {
                return null;
            }
            return result.Datasources.Datasource.First();
        }

        /// <summary>
        /// Runs an extract refresh on the specified datasource.
        /// </summary>
        /// <param name="name">datasource name</param>
        /// <returns></returns>
        public RefreshDatasourceResponse.Job RefreshDatasource(string name)
        {
            var datasource = GetDatasource(name);

            string url = $"/api/{apiVersion}/sites/{siteId}/datasources/{datasource.Id}/refresh";

            string request = $"<tsRequest></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<RefreshDatasourceResponse.TsResponse>();
            if (result.Job == null)
            {
                return null;
            }
            return result.Job;
        }
    }
}

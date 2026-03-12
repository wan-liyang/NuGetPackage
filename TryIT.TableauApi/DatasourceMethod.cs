using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<GetDataSourceResponse.Datasource> GetDatasource(string name)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/datasources?filter=name:eq:{name}";

            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetDataSourceResponse.TsResponse>();
            if (result.Datasources.Datasource == null)
            {
                return null;
            }
            return result.Datasources.Datasource[0];
        }

        /// <summary>
        /// Runs an extract refresh on the specified datasource.
        /// </summary>
        /// <param name="name">datasource name</param>
        /// <returns></returns>
        public async Task <RefreshDatasourceResponse.Job> RefreshDatasource(string name)
        {
            var datasource = await GetDatasource(name);

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/datasources/{datasource.Id}/refresh";

            string request = $"<tsRequest></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<RefreshDatasourceResponse.TsResponse>();
            if (result.Job == null)
            {
                return null;
            }
            return result.Job;
        }
    }
}

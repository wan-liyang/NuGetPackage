using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.SiteList;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// sharepoint list api, use to opoerate on sharepoint list
    /// </summary>
    public class SiteListApi
    {
        private readonly SiteListHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config">configuration for api request, e.g token, timeout, proxy etc</param>
        /// <param name="hostName">the host(domain) of the site, use for api request to get site under specific host</param>
        public SiteListApi(MsGraphApiConfig config, string hostName)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SiteListHelper(graphHelper.GetHttpClient(), hostName);
        }

        /// <summary>
        /// get all list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public List<GetListResponse.SiteList> GetAllList(string siteName)
        {
            return _helper.GetAllList(siteName);
        }


        /// <summary>
        /// get a specific list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public GetListResponse.SiteList GetList(string siteName, string listName)
        {
            return _helper.GetList(siteName, listName);
        }

        /// <summary>
        /// get items with default columns, no other customize column
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public List<GetItemResponse.Item> GetItems(string siteName, string listName)
        {
            return _helper.GetItems(siteName, listName);
        }

        /// <summary>
        /// get items as a entity list from a sharepoint list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siteId"></param>
        /// <param name="listId"></param>
        /// <param name="expression">the expression to filter, e.g. title eq 'xxx', give null if do not apply filter</param>
        /// <returns></returns>
        public async Task<List<T>> GetItemsAsync<T>(string siteId, string listId, string expression) where T : class
        {
            return await _helper.GetItemsAsync<T>(siteId, listId, expression);
        }

        /// <summary>
        /// get specific item from list, convert result as expected object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetItem<T>(string siteName, string listName, string id)
        {
            return _helper.GetItem<T>(siteName, listName, id);
        }

        /// <summary>
        /// create a item into list, refer to https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public GetItemResponse.Item CreateItem(string siteName, string listName, string jsonBody)
        {
            return _helper.CreateItem(siteName, listName, jsonBody);
        }



        /// <summary>
        /// update a item, refer to https://learn.microsoft.com/en-us/graph/api/listitem-update
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public GetItemResponse.Fields UpdateItem(string siteName, string listName, string itemId, string jsonBody)
        {
            return _helper.UpdateItem(siteName, listName, itemId, jsonBody);
        }
    }
}

﻿using System.Collections.Generic;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.SiteList;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    public class SiteListApi
    {
        private SiteListHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="hostName"></param>
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

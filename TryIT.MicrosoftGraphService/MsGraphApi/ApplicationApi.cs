using System;
using System.Collections.Generic;
using System.Text;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.MsGraphApi
{
    /// <summary>
    /// application api
    /// </summary>
    public class ApplicationApi
    {
        private static ApplicationHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public ApplicationApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new ApplicationHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get application
        /// </summary>
        /// <param name="appDisplayName">application display name</param>
        /// <returns></returns>
        public ApplicationModel GetApplication(string appDisplayName)
        {
            var app = _helper.GetApplication(appDisplayName);

            return app.ToApplicationModel();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.SiteModel
{
    /// <summary>
    /// API request information model
    /// </summary>
    public class ApiRequestModel
    {
        /// <summary>
        /// Tableau host url to make api request
        /// </summary>
        public string HostUrl { get; set; }
        /// <summary>
        /// Tableau site name
        /// </summary>
        public string Sitename { get; set; }
        /// <summary>
        /// Tableau api version, https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_versions.htm
        /// </summary>
        public string ApiVersion { get; set; }
        /// <summary>
        /// Tableau token name
        /// </summary>
        public string TokenName { get; set; }
        /// <summary>
        /// Tableau token secret
        /// </summary>
        public string TokenSecret { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;

namespace RestApiService
{
    public class ApiRequest
    {
        /// <summary>
        /// request Url contains parameter if parameter pass via Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// username for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Username { get; set; }

        private static SecureString secureString;
        /// <summary>
        /// password for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Password
        {
            get
            {
                return new System.Net.NetworkCredential(string.Empty, secureString).Password;
            }
            set
            {
                secureString = new NetworkCredential(string.Empty, value).SecurePassword;
            }
        }

        public string Body { get; set; }

        public WebProxyInfo WebProxy { get; set; }
    }

    public class WebProxyInfo
    {
        /// <summary>
        /// Url for proxy server
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Username for connect to proxy, if empty will set <see cref="WebProxy.UseDefaultCredentials"/> to true
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password for connect to proxy
        /// </summary>
        public string Password { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;

namespace HttpHandler
{
    /// <summary>
    /// initial configuration of HttpHandler
    /// </summary>
    public class Config
    {
        internal static IHttpContextAccessor _httpContextAccessor;
        internal static readonly string QueryKey_Param = "param";
        internal static string QueryStringPassword
        {
            get
            {
                return string.IsNullOrEmpty(_queryPassword) ? "e7v_32kpYp" : _queryPassword;
            }
        }
        internal static IHttpContextAccessor m_httpContextAccessor
        {
            get
            {
                if (_httpContextAccessor == null)
                {
                    throw new ArgumentNullException("httpContextAccessor is null, please call Configure method in Startup.cs");
                }

                return _httpContextAccessor;
            }
        }
        private static string _queryPassword = string.Empty;

        /// <summary>
        /// config <see cref="IHttpContextAccessor"/>
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// config encrypt password, use to encrypt/decrypt query string
        /// </summary>
        /// <param name="urlQueryEncryptPassword"></param>
        public static void ConfigurePassword(string urlQueryEncryptPassword)
        {
            _queryPassword = urlQueryEncryptPassword;
        }
    }
}

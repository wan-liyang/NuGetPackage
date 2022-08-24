using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using TryIT.CryptoService;
using TryIT.ObjectExtension;

namespace TryIT.HttpHandler
{
    /// <summary>
    /// helper for URL related
    /// </summary>
    public partial class CurrentRequest
    {
        private static class HttpContext
        {
            public static Microsoft.AspNetCore.Http.HttpContext Current
            {
                get
                {
                    return Config.m_httpContextAccessor.HttpContext;
                }
            }
        }

        public static HttpRequest Request
        {
            get
            {
                return CurrentRequest.HttpContext.Current.Request;
            }
        }

        #region Request Static Info (ClientIP, UserAgent, SessionId)
        /// <summary>
        /// get client ip address, return empty if not applicable
        /// </summary>
        public static string ClientIPAddress
        {
            get
            {
                string clientIP = string.Empty;
                try
                {
                    if (HttpContext.Current != null
                        && HttpContext.Current.Connection != null
                        && HttpContext.Current.Connection.RemoteIpAddress != null)
                    {
                        clientIP = HttpContext.Current.Connection.RemoteIpAddress.ToString();
                    }
                }
                catch
                {
                }
                return clientIP;
            }
        }
        /// <summary>
        /// get client user agent, return empty if not applicable
        /// </summary>
        public static string UserAgent
        {
            get
            {
                string userAgent = string.Empty;
                try
                {
                    if (HttpContext.Current != null
                        && HttpContext.Current.Request != null)
                    {
                        userAgent = HttpContext.Current.Request.Headers["User-Agent"].ToString();
                    }
                }
                catch
                {
                }
                return userAgent;
            }
        }

        /// <summary>
        /// get SessionId for current request, return empty if not applicable
        /// </summary>
        public static string SessionId
        {
            get
            {
                string result = string.Empty;
                try
                {
                    if (HttpContext.Current != null
                        && HttpContext.Current.Session != null
                        && HttpContext.Current.Session.Id != null)
                    {
                        result = HttpContext.Current.Session.Id;
                    }
                }
                catch
                {
                }
                return result;
            }
        }
        #endregion

        /// <summary>
        /// {Scheme}://{Host}:{Port}/{AppPath}/{Page}?{Query}
        /// </summary>
        public class Url
        {
            /// <summary>
            /// get Scheme or current request, "http" / "https"
            /// </summary>
            public static string Scheme
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.Scheme != null)
                        {
                            result = HttpContext.Current.Request.Scheme;
                        }
                    }
                    catch { }

                    return result;
                }
            }

            /// <summary>
            /// port of current request, 443 / 80
            /// </summary>
            public static int Port
            {
                get
                {
                    int result = 0;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Connection != null)
                        {
                            result = HttpContext.Current.Connection.LocalPort;
                        }
                    }
                    catch { }

                    return result;
                }
            }

            /// <summary>
            /// get host (with scheme, with port) of current request, return empty if not http request (e.g. backend schedule job)
            /// </summary>
            public static string Host
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.Host.Host != null)
                        {
                            result = string.Format("{0}://{1}", Scheme, HttpContext.Current.Request.Host.Value);
                        }
                    }
                    catch { }
                    return result;
                }
            }

            /// <summary>
            /// get Reqeust Application Path, return empty if it's "/"
            /// </summary>
            public static string AppPath
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.PathBase != null)
                        {
                            result = HttpContext.Current.Request.PathBase;

                            if (result.EndsWith("/"))
                            {
                                result = result.TrimEnd('/');
                            }
                        }
                    }
                    catch { }
                    return result;
                }
            }

            /// <summary>
            /// e.g. https://host:443/AppPath or https://host:443
            /// </summary>
            public static string HostAndAppPath
            {
                get
                {
                    return string.Concat(Host, AppPath);
                }
            }

            /// <summary>
            /// query without '?'
            /// <para>e.g. a=1&amp;b=2</para>
            /// </summary>
            public static string Query
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.Query != null)
                        {
                            result = HttpContext.Current.Request.QueryString.Value;
                        }
                    }
                    catch { }
                    return result.TrimStart('?');
                }
            }

            /// <summary>
            /// querystring collection
            /// </summary>
            public static IQueryCollection QueryCollection
            {
                get
                {
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.Query != null)
                        {
                            return HttpContext.Current.Request.Query;
                        }
                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// e.g. /DEV/page.aspx
            /// </summary>
            public static string Page
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null
                            && HttpContext.Current.Request.Path != null)
                        {
                            result = HttpContext.Current.Request.Path;
                        }
                    }
                    catch { }
                    return result;
                }
            }

            /// <summary>
            /// full url with query (if have), {Scheme}://{Host}:{Port}/{AppPath}/{Page}?{Query}
            /// <para>Returns the combined components of the request URL in a fully un-escaped form (except for the QueryString) suitable only for display. This format should not be used in HTTP headers or other HTTP operations.</para>
            /// </summary>
            public static string FullDisplayUrl
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null)
                        {
                            result = HttpContext.Current.Request.GetDisplayUrl();
                        }
                    }
                    catch { }
                    return result;
                }
            }

            /// <summary>
            /// Returns the combined components of the request URL in a fully escaped form suitable for use in HTTP headers and other HTTP operations.
            /// </summary>
            public static string FulEncodedUrl
            {
                get
                {
                    string result = string.Empty;
                    try
                    {
                        if (HttpContext.Current != null
                            && HttpContext.Current.Request != null)
                        {
                            result = HttpContext.Current.Request.GetEncodedUrl();
                        }
                    }
                    catch { }
                    return result;
                }
            }

            /// <summary>
            /// full url without query, {Scheme}://{Host}:{Port}/{AppPath}/{Page}
            /// </summary>
            public static string FullUrlWithoutQuery
            {
                get
                {
                    return string.Concat(HostAndAppPath, Page);
                }
            }
        }


        /// <summary>
        /// based on current request, get absolute url based on given url and query string list, querystring will be encrypted
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dicQueryString">query string dictionary</param>
        /// <returns></returns>
        public static string GetAbsoluteUrl(string url, Dictionary<string, string> dicQueryString = null)
        {
            if (url.StartsWith(Url.HostAndAppPath))
            {
                return url;
            }
            if (url.StartsWith("~"))
            {
                url = url.Remove(0, 1);
            }
            if (!url.StartsWith("/"))
            {
                url = "/" + url;
            }

            url = ConcatUrlAndQuery(url, dicQueryString, true);

            return Url.HostAndAppPath + url;
        }

        /// <summary>
        /// Concat Url and Query into String
        /// </summary>
        /// <param name="url">base url</param>
        /// <param name="dicQueryString">query string</param>
        /// <param name="isEncryptQuery">indicator whether encrypt query string, default false</param>
        /// <returns></returns>
        internal static string ConcatUrlAndQuery(string url, Dictionary<string, string> dicQueryString = null, bool isEncryptQuery = false)
        {
            if (dicQueryString != null && dicQueryString.Count > 0)
            {
                var stringAppend = new StringAppend();
                foreach (var item in dicQueryString)
                {
                    string value = string.IsNullOrEmpty(item.Value) ? string.Empty : item.Value.ToString();
                    stringAppend.Append($"{item.Key}={value}");
                }
                string strQueryString = stringAppend.ToString("&");

                if (isEncryptQuery)
                {
                    string cipherQueryString = AESEncryption.AESEncrypt(strQueryString, Config.QueryStringPassword, true);

                    // if url already not contains query string, then conact by "?", otherwise concat by "&"
                    string concat = "?";
                    if (url.IsContains(concat))
                    {
                        concat = "&";
                    }
                    url = $"{url}{concat}{Config.QueryKey_Param}={cipherQueryString}";
                }
                else
                {
                    string concat = "?";
                    if (url.IsContains(concat))
                    {
                        concat = "&";
                    }
                    url = $"{url}{concat}{strQueryString}";
                }
            }
            return url;
        }

        /// <summary>
        /// Get QueryStringValue from current request based on <paramref name="queryStringKey"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryStringKey"></param>
        /// <returns></returns>
        public static T GetQueryValue<T>(string queryStringKey)
        {
            string value = string.Empty;

            var queryCollection = Url.QueryCollection;

            //NameValueCollection queryCollection = Url.QueryCollection;
            if (queryCollection != null && queryCollection.Count >= 0)
            {
                bool isFound = false;
                string cipherQueryString = queryCollection[Config.QueryKey_Param];

                // if exists <see cref="QueryKey_Param"/>, means QueryString is encrypted
                if (!string.IsNullOrEmpty(cipherQueryString))
                {
                    string clearText = AESEncryption.AESDecrypt(cipherQueryString, Config.QueryStringPassword, true);
                    string[] arrVariables = clearText.Split('&', '=');

                    // "key1", "value1", "key2", "value2", "key3", "value3" ...
                    int keyIndex = arrVariables.IndexOf(queryStringKey);

                    // key index must > -1 and belong to 0, 2, 4, ...
                    if (keyIndex > -1 && ((keyIndex % 2) == 0))
                    {
                        isFound = true;
                        // value index is key index + 1
                        value = arrVariables[keyIndex + 1];
                    }
                }

                // if <paramref name="queryStringKey"/> not exists in encrypted QueryString, then try get from query string directly
                if (!isFound)
                {
                    value = queryCollection[queryStringKey];
                }
            }

            return value.ConvertValue<T>();
        }

        /// <summary>
        /// indicator current request is requesting specific page <paramref name="pageUrl"/>
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public static bool IsPage(string pageUrl)
        {
            string currentUrl = Url.FullUrlWithoutQuery;
            string provideUrl = GetAbsoluteUrl(pageUrl);
            return currentUrl.IsEquals(provideUrl);
        }
    }
}

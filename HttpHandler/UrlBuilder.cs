using System.Collections.Generic;

namespace HttpHandler
{
    /// <summary>
    /// to build Absolute url with given url and parametr
    /// <para>Example Code</para>
    /// <para>
    /// string absUrl = new UrlBuilder().Url("~/test.aspx").Parameter("Param1", "Value1").Parameter("Param2", "Value2").ToString();
    /// </para>
    /// </summary>
    public class UrlBuilder
    {
        private string _host = string.Empty;
        private string _url = string.Empty;
        private bool _isEncrypt = false;
        private Dictionary<string, string> _param = null;

        /// <summary>
        /// the page to present in url
        /// </summary>
        /// <param name="page">the page use to build url</param>
        /// <returns></returns>
        public UrlBuilder Url(string page)
        {
            _url = page;
            return this;
        }

        /// <summary>
        /// the host to present in url
        /// </summary>
        /// <returns></returns>
        public UrlBuilder Host(string host)
        {
            _host = host;
            return this;
        }

        /// <summary>
        /// the query parameter to append into url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlBuilder Parameter(string key, string value)
        {
            if (_param == null)
            {
                _param = new Dictionary<string, string>();
            }

            _param[key] = value;
            return this;
        }

        /// <summary>
        /// indicator whether encrypt query parameter, default false
        /// <para>encrypt password refer to <see cref="Config.ConfigurePassword(string)" /></para>
        /// </summary>
        /// <param name="isEncryptParameter"></param>
        /// <returns></returns>
        public UrlBuilder Encrypt(bool isEncryptParameter)
        {
            _isEncrypt = isEncryptParameter;
            return this;
        }

        /// <summary>
        /// get final url string, 
        /// <para>if host provied, will be host + page + parameter</para>
        /// <para>if host not provied, will be page + parameter</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string url = _url;
            url = CurrentRequest.ConcatUrlAndQuery(url, _param, _isEncrypt);

            if (string.IsNullOrEmpty(_host))
            {
                return url;
            }
            else
            {
                string host = _host;

                if (host.EndsWith("/"))
                {
                    host = host.TrimEnd('/');
                }

                if (url.StartsWith("~"))
                {
                    url = url.Remove(0, 1);
                }

                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }

                return host + url;
            }
        }
    }
}

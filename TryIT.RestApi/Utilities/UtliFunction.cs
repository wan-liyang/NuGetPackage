using System.Net;
using TryIT.RestApi.Models;

namespace TryIT.RestApi.Utilities
{
    internal static class UtliFunction
    {
        internal static WebProxy GetWebProxy(ProxyConfig webProxyInfo)
        {
            if (!string.IsNullOrEmpty(webProxyInfo.Url))
            {
                WebProxy proxy = new WebProxy(webProxyInfo.Url);
                proxy.UseDefaultCredentials = true;

                if (!string.IsNullOrEmpty(webProxyInfo.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(webProxyInfo.Username, webProxyInfo.Password);
                }
                proxy.BypassProxyOnLocal = true;

                return proxy;
            }
            return null;
        }
    }
}

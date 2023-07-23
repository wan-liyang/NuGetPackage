using System.Net;

namespace TryIT.MicrosoftGraphApi.Helper
{
    internal class WebProxyHelper
    {
        public static WebProxy GetProxy(string url, string username = "", string password = "")
        {
            WebProxy proxy = null;

            if (!string.IsNullOrEmpty(url))
            {
                proxy = new WebProxy(url);

                if (!string.IsNullOrEmpty(username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(username, password);
                }
                proxy.BypassProxyOnLocal = true;
            }

            return proxy;
        }
    }
}

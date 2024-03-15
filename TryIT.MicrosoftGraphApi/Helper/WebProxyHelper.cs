using System.Net;
using TryIT.MicrosoftGraphApi.Model.Utility;

namespace TryIT.MicrosoftGraphApi.Helper
{
    internal class WebProxyHelper
    {
        public static WebProxy GetProxy(ProxyModel proxyModel)
        {
            WebProxy proxy = null;

            if (proxyModel != null && !string.IsNullOrEmpty(proxyModel.Url))
            {
                proxy = new WebProxy(proxyModel.Url);

                if (!string.IsNullOrEmpty(proxyModel.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(proxyModel.Username, proxyModel.Password);
                }
                proxy.BypassProxyOnLocal = true;
            }

            return proxy;
        }
    }
}

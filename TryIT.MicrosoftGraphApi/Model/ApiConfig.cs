using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.Model.Utility;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// (required) information to obtaion token
        /// </summary>
        public GetTokenModel TokenRequestInfo { get; set; }

        /// <summary>
        /// (optional) proxy to make api call
        /// </summary>
        public ProxyModel Proxy { get; set; }
        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }
    }
}

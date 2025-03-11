using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.Model.Utility;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiConfig : BaseApiConfig
    {
        /// <summary>
        /// (required) information to obtaion token
        /// </summary>
        public GetTokenModel TokenRequestInfo { get; set; }
    }
}

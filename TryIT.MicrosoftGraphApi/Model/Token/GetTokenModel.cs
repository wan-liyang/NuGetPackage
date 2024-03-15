namespace TryIT.MicrosoftGraphApi.Model.Token
{
    /// <summary>
    /// required information to obtain token
    /// </summary>
    public class GetTokenModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string tenant_id { get; set; }
        /// <summary>
        /// client_credentials
        /// </summary>
        public string grant_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string client_secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scope { get; set; }
    }
}

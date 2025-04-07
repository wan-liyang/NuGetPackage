namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// azure key vault provide information
    /// </summary>
    public class AzureServicePrincipal
    {
        /// <summary>
        /// tenant id
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// proxy url to set environment variable, to solve Retry failed after 4 tries. 
        /// <para>Retry settings can be adjusted in ClientOptions.Retry. (The SSL connection could not be established) accessing keyvault</para> 
        /// </summary>
        public string ProxyUrl { get; set; }
    }
}

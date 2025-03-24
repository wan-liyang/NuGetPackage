using System.Net;
using System.Security;

namespace TryIT.RestApi.Models
{
    /// <summary>
    /// basic auth proper
    /// </summary>
    public class BasicAuth
    {
        /// <summary>
        /// username for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Username { get; set; }

        private static SecureString secureString;
        /// <summary>
        /// password for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Password
        {
            get
            {
                return new NetworkCredential(string.Empty, secureString).Password;
            }
            set
            {
                secureString = new NetworkCredential(string.Empty, value).SecurePassword;
            }
        }
    }
}

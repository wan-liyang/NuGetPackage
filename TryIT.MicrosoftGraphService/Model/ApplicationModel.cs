using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.Model
{
    /// <summary>
    /// azure application information
    /// </summary>
    public class ApplicationModel
    {
        /// <summary>
        /// application object id
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// application deleted time
        /// </summary>
        public object DeletedDateTime { get; set; }

        /// <summary>
        /// application id, aka Client ID
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// application created time
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// application display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// application client secrets
        /// </summary>
        public List<ClientSecret> ClientSecrets { get; set; }


        /// <summary>
        /// client secret information
        /// </summary>
        public class ClientSecret
        {
            /// <summary>
            /// secret display name
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// secret expiry time
            /// </summary>
            public DateTime EndDateTime { get; set; }

            /// <summary>
            /// secret id
            /// </summary>
            public string SecretId { get; set; }

            /// <summary>
            /// secret start time
            /// </summary>
            public DateTime StartDateTime { get; set; }
        }
    }
}

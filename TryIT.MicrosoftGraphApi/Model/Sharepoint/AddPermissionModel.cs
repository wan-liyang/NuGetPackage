using System;
using System.Collections.Generic;
using System.Text;
using static TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest;

namespace TryIT.MicrosoftGraphApi.Model.Sharepoint
{
    public class AddPermissionModel
    {
        public List<Recipient> recipients { get; set; }

        /// <summary>
        /// The alias of the domain object, for cases where an email address is unavailable (for example, security groups).
        /// </summary>
        public string alias { get; set; }

        /// <summary>
        /// The email address for the recipient, if the recipient has an associated email address.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// The unique identifier for the recipient in the directory.
        /// </summary>
        public string objectId { get; set; }

        public bool sendInvitation { get; set; }
        public PermissionRoleEnum role { get; set; }
    }

    public enum PermissionRoleEnum
    {
        read,
        write
    }
}

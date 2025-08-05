using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    /// <summary>
    /// add permission request for SharePoint items.
    /// </summary>
    public class AddPermissionRequest
    {
        /// <summary>
        /// Represents a person, group, or other recipient to share a drive item with using the invite action.
        /// <para>When using invite to add permissions, the driveRecipient object would specify the email, alias, or objectId of the recipient.Only one of these values is required; multiple values are not accepted.</para>
        /// <para>https://learn.microsoft.com/en-us/graph/api/resources/driverecipient?view=graph-rest-1.0</para>
        /// </summary>
        public class Recipient
        {
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
        }

        /// <summary>
        /// request body for adding permissions to a SharePoint item.
        /// </summary>
        public class Body
        {
            /// <summary>
            /// A collection of recipients who receive access and the sharing invitation.
            /// </summary>
            public List<Recipient> recipients { get; set; }

            //public string message { get; set; }

            /// <summary>
            /// Default true, specifies whether the recipient of the invitation is required to sign-in to view the shared item.
            /// </summary>
            public bool requireSignIn { get; set; } = true;

            /// <summary>
            /// Default false, if true, a sharing link is sent to the recipient. Otherwise, a permission is granted directly without sending a notification.
            /// </summary>
            public bool sendInvitation { get; set; } = false;

            /// <summary>
            /// Specifies the roles that are to be granted to the recipients of the sharing invitation.
            /// </summary>
            public List<PermissionRole> roles { get; set; }

            //public string password { get; set; }
            //public string expirationDateTime { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum PermissionRole
        {
            /// <summary>
            /// read permission
            /// </summary>
            read,

            /// <summary>
            /// write permission
            /// </summary>
            write
        }
    }
}

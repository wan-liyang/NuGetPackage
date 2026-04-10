using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    /// <summary>
    /// Create sharing link request for SharePoint items.
    /// </summary>
    public class CreateLinkRequest
    {
        /// <summary>
        /// Represents the request body for creating a sharing link via Microsoft Graph API.
        /// Used in POST /drives/{drive-id}/items/{item-id}/createLink
        /// </summary>
        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class Body
        {
            /// <summary>
            /// The type of sharing link to create.
            /// Possible values: "view", "edit", "embed".
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// The password for the sharing link. 
            /// <para>Optional. Supported only on OneDrive Personal.</para>
            /// </summary>
            public string password { get; set; }

            /// <summary>
            /// The expiration date and time of the sharing link.
            /// <para>Format: yyyy-MM-ddTHH:mm:ssZ (UTC ISO 8601).</para>
            /// <para>Example: "2026-12-31T23:59:59Z"</para>
            /// </summary>
            public string expirationDateTime { get; set; }

            /// <summary>
            /// Determines whether inherited permissions are retained when sharing the item for the first time.
            /// <para>If true (default), existing inherited permissions are kept.</para>
            /// <para>If false, all existing permissions are removed when sharing for the first time.</para>
            /// </summary>
            public bool retainInheritedPermissions { get; set; } = true;

            /// <summary>
            /// The scope of the sharing link.
            /// <para>Possible values: "anonymous", "organization", "users".</para>
            /// <para>Optional – if not provided, defaults to the most restrictive appropriate scope.</para>
            /// </summary>
            public string scope { get; set; }
        }
    }
}

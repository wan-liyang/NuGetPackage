using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    /// <summary>
    /// Get a set of messages that have been added, deleted, or updated in a specified folder.
    /// <para>https://learn.microsoft.com/en-us/graph/api/message-delta?view=graph-rest-1.0&tabs=http</para>
    /// </summary>
    public class GetDeltaMessage : MessageBaseModel
    {
        /// <summary>
        /// folder name, refer to here for well-know folder name
        /// <para>https://learn.microsoft.com/en-us/graph/api/resources/mailfolder?view=graph-rest-1.0</para>
        /// </summary>
        public string folder { get; set; }

        /// <summary>
        /// the URL to use to get the next page of results, or null if there are no additional pages.
        /// </summary>
        public string odatanextLink { get; set; }

        /// <summary>
        /// the URL to use to get the next set of changes, or null if a full resynchronization is required.
        /// </summary>
        public string odatadeltaLink { get; set; }
    }
}

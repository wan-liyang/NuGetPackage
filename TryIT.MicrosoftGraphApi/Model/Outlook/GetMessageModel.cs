using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    public class GetMessageModel : MessageBaseModel
    {
        /// <summary>
        /// folder name, refer to here for well-know folder name
        /// <para>https://learn.microsoft.com/en-us/graph/api/resources/mailfolder?view=graph-rest-1.0</para>
        /// </summary>
        public string folder { get; set; }

        /// <summary>
        /// get number of message
        /// </summary>
        public int top { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    /// <summary>
    /// get mailbox folders
    /// </summary>
    public class GetMailboxFolderModel : MessageBaseModel
    {
        /// <summary>
        /// the expression to filter folder, it will append to query as $filter=<see cref="filterExpression"/>
        /// </summary>
        public string filterExpression { get; set; }
    }
}

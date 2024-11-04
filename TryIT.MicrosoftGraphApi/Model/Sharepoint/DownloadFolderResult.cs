using System;
using System.Collections.Generic;
using System.Text;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.Model.Sharepoint
{
    public class DownloadFolderResult
    {
        /// <summary>
        /// indicate is file or folder
        /// </summary>
        public string FileOrFolder { get; set; }
        public GetDriveItemResponse.Item DriveItem { get; set; }
        public string LocalItem { get; set; }
    }
}

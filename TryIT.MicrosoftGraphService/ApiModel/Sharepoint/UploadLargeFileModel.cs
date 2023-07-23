using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Sharepoint
{
    internal class UploadLargeFileModel
    {
        public string DriveId { get; set; }
        public string ItemId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}

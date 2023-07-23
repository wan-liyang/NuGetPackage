using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Sharepoint
{
    internal class UploadSmallFileModel
    {
        public string SiteId { get; set; }
        public string ItemId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileId { get; set; }
    }
}

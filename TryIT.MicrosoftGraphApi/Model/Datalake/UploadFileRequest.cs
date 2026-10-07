using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Datalake
{
    public class UploadFileRequest : BaseRequest
    {
        public string FilePath { get; set; }
        public System.IO.Stream FileStream { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Datalake
{
    public class GetFileRequest : BaseRequest
    {
        public string FilePath { get; set; }
    }
}

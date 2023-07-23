using System;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    internal class CreateUploadSessionResponse
    {
        public class Response
        {
            public string uploadUrl { get; set; }
            public DateTime expirationDateTime { get; set; }
        }
    }
}

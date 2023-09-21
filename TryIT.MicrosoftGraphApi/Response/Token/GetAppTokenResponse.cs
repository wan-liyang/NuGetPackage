using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Token
{
    public class GetAppTokenResponse
    {
        public class Response
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
        }
    }
}

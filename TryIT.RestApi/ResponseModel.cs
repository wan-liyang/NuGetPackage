using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TryIT.RestApi
{
    public class ResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpContent Content { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RestApiService
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpContent Content { get; set; }
    }
}

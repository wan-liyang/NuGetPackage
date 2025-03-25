using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Utlity
{
    internal class ServiceNotAvailableResponse
    {
        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
            public int retryAfterSeconds { get; set; }
        }

        public class Response
        {
            public Error error { get; set; }
        }
    }
}

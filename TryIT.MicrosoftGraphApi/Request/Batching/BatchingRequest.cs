using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Batching
{
    public class BatchingRequest
    {
        public class Body
        {
            public Body()
            {
                requests = new List<Request>();
            }

            public List<Request> requests { get; set; }
        }
        public class Request
        {
            public string id { get; set; }
            public string method { get; set; }
            public string url { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Datalake
{
    public class ListFileResponse
    {
        public List<Path> paths { get; set; }

        public class Path
        {
            public string contentLength { get; set; }
            public string etag { get; set; }
            public string lastModified { get; set; }
            public string name { get; set; }
        }
    }
}

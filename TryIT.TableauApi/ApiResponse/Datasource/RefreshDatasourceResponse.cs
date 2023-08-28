using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse.Datasource
{
    public class RefreshDatasourceResponse
    {
        public class Datasource
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class ExtractRefreshJob
        {
            public Datasource Datasource { get; set; }
        }

        public class Job
        {
            public ExtractRefreshJob ExtractRefreshJob { get; set; }
            public string Id { get; set; }
            public string Mode { get; set; }
            public string Type { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class TsResponse
        {
            public Job Job { get; set; }
            public string Xmlns { get; set; }
            public string Xsi { get; set; }
            public string SchemaLocation { get; set; }
        }
    }
}

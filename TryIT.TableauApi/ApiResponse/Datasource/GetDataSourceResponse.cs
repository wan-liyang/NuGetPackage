using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TryIT.TableauApi.ApiResponse.Datasource
{
    public class GetDataSourceResponse
    {
        public class Pagination
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalAvailable { get; set; }
        }

        public class Project
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Owner
        {
            public string Id { get; set; }
        }

        public class Datasource
        {
            public Project Project { get; set; }
            public Owner Owner { get; set; }
            public object Tags { get; set; }
            public string ContentUrl { get; set; }
            public DateTime CreatedAt { get; set; }
            public object Description { get; set; }
            public bool EncryptExtracts { get; set; }
            public bool HasExtracts { get; set; }
            public string Id { get; set; }
            public bool IsCertified { get; set; }
            public string Name { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool UseRemoteQueryAgent { get; set; }
            public string WebpageUrl { get; set; }
        }

        public class Datasources
        {
            public List<Datasource> Datasource { get; set; }
        }

        public class TsResponse
        {
            public Pagination Pagination { get; set; }
            public Datasources Datasources { get; set; }
            public string Xmlns { get; set; }
            public string Xsi { get; set; }
            public string SchemaLocation { get; set; }
        }
    }
}

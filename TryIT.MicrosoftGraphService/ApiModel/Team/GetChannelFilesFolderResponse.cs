using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TryIT.MicrosoftGraphService.Model;
using TryIT.MicrosoftGraphService.Model.Team;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class GetChannelFilesFolderResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public int size { get; set; }
            public ParentReference parentReference { get; set; }
            public FileSystemInfo fileSystemInfo { get; set; }
            public Folder folder { get; set; }
        }
        public class FileSystemInfo
        {
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
        }

        public class Folder
        {
            public int childCount { get; set; }
        }

        public class ParentReference
        {
            public string driveId { get; set; }
            public string driveType { get; set; }
        }
    }

    internal static class GetChannelFilesFolderResponseExtension
    {
        public static ChannelSharepointModel ToChannelSharepointModel(this GetChannelFilesFolderResponse.Response response)
        {
            ChannelSharepointModel model = new ChannelSharepointModel();
            if (response != null)
            {
                model.WebUrl = response.webUrl;
            }
            return model;
        }
    }
}

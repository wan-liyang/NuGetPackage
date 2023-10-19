using Newtonsoft.Json;

namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    internal class RenameItemRequest
    {
        internal class Body
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}

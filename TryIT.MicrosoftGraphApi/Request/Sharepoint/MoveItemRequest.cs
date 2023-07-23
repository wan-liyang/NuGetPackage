namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    internal class MoveItemRequest
    {
        internal class Body
        {
            public ParentReference parentReference { get; set; }
            public string name { get; set; }
        }

        internal class ParentReference
        {
            public string id { get; set; }
        }
    }
}

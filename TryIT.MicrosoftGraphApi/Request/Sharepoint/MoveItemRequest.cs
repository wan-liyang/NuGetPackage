namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    public class MoveItemRequest
    {
        public class Body
        {
            public ParentReference parentReference { get; set; }
            public string name { get; set; }
        }

        public class ParentReference
        {
            public string id { get; set; }
        }
    }
}

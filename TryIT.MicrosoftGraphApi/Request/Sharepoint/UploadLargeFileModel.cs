namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    internal class UploadLargeFileModel
    {
        public string DriveId { get; set; }
        public string ItemId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}

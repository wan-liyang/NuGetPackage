namespace TryIT.MicrosoftGraphService.Model
{
    public class SharepointModel
    {
        public class Site
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }

        public class SiteUploadFileModel
        {
            public string SiteId { get; set; }
            public string DriveItemId { get; set; }
            public string FileName { get; set; }
            public byte[] FileContent { get; set; }
        }

        public class SiteDriveItemModel
        {
            public string Id { get; set; }
            public string Name { get; set; }

            /// <summary>
            /// true: is file object, falsse: is folder object
            /// </summary>
            public bool IsFile { get; set; }

            /// <summary>
            /// the web url of the file or folder
            /// </summary>
            public string WebUrl { get; set; }

            public SiteDriveItemModel ParentDriveItem { get; set; }
        }

        public class SiteDriveItemPreviewModule
        {
            public string Url { get; set; }
        }
    }
}

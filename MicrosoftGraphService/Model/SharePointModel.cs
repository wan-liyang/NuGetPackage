﻿namespace MicrosoftGraphService.Model
{
    public class SharePointModel
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

            public SiteDriveItemModel ParentDriveItem { get; set; }
        }

        public class SiteDriveItemPreviewModule
        {
            public string Url { get; set; }
        }
    }
}

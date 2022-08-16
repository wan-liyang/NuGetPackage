namespace TryIT.MicrosoftGraphService.Model
{
    public static class SharePointResponseExt
    {
        public static SharePointModel.Site ToSiteModule(this SharePointResponse.GetSiteResponse site)
        {
            SharePointModel.Site module = new SharePointModel.Site();
            if (site != null)
            {
                module.Id = site.id;
                module.Name = site.name;
                module.DisplayName = site.displayName;
            }
            return module;
        }

        public static SharePointModel.SiteDriveItemModel ToSiteDriveItem(this SharePointResponse.GetDriveItemResponse driveItem)
        {
            SharePointModel.SiteDriveItemModel module = new SharePointModel.SiteDriveItemModel();
            if (driveItem != null)
            {
                module.Id = driveItem.id;
                module.Name = driveItem.name;
            }
            return module;
        }

        public static SharePointModel.SiteDriveItemPreviewModule ToSiteDriveItemPreviewModule(this SharePointResponse.GetDriveItemPreviewResponse driveItem)
        {
            SharePointModel.SiteDriveItemPreviewModule module = new SharePointModel.SiteDriveItemPreviewModule();
            if (driveItem != null)
            {
                module.Url = driveItem.getUrl;
            }
            return module;
        }
    }
}

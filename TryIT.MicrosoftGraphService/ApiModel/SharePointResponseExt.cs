using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class SharePointResponseExt
    {
        public static SharepointModel.Site ToSiteModule(this SharePointResponse.GetSiteResponse site)
        {
            SharepointModel.Site module = new SharepointModel.Site();
            if (site != null)
            {
                module.Id = site.id;
                module.Name = site.name;
                module.DisplayName = site.displayName;
            }
            return module;
        }

        public static SharepointModel.SiteDriveItemModel ToSiteDriveItem(this SharePointResponse.GetDriveItemResponse driveItem)
        {
            SharepointModel.SiteDriveItemModel module = new SharepointModel.SiteDriveItemModel();
            if (driveItem != null)
            {
                module.Id = driveItem.id;
                module.Name = driveItem.name;
            }
            return module;
        }

        public static SharepointModel.SiteDriveItemPreviewModule ToSiteDriveItemPreviewModule(this SharePointResponse.GetDriveItemPreviewResponse driveItem)
        {
            SharepointModel.SiteDriveItemPreviewModule module = new SharepointModel.SiteDriveItemPreviewModule();
            if (driveItem != null)
            {
                module.Url = driveItem.getUrl;
            }
            return module;
        }
    }
}

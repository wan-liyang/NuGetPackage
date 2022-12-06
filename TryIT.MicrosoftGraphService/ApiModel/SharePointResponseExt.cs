using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class SharePointResponseExt
    {
        public static SharepointModel.Site ToSiteModule(this SharePointResponse.GetSiteResponse site)
        {
            SharepointModel.Site model = new SharepointModel.Site();
            if (site != null)
            {
                model.Id = site.id;
                model.Name = site.name;
                model.DisplayName = site.displayName;
            }
            return model;
        }

        public static SharepointModel.SiteDriveItemModel ToSiteDriveItem(this SharePointResponse.GetDriveItemResponse driveItem)
        {
            SharepointModel.SiteDriveItemModel model = new SharepointModel.SiteDriveItemModel();
            if (driveItem != null)
            {
                model.Id = driveItem.id;
                model.Name = driveItem.name;
                model.IsFile = driveItem.file != null;
                model.WebUrl = driveItem.webUrl;
            }
            return model;
        }

        public static SharepointModel.SiteDriveItemPreviewModule ToSiteDriveItemPreviewModule(this SharePointResponse.GetDriveItemPreviewResponse driveItem)
        {
            SharepointModel.SiteDriveItemPreviewModule model = new SharepointModel.SiteDriveItemPreviewModule();
            if (driveItem != null)
            {
                model.Url = driveItem.getUrl;
            }
            return model;
        }
    }
}

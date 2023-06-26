using TryIT.MicrosoftGraphService.Model;
using static TryIT.MicrosoftGraphService.ApiModel.GroupResponse;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class GroupResponseExt
    {
        public static GroupModel ToGroupModel(this GetGroupResponse response)
        {
            if (response == null || response.value == null || response.value.Count == 0)
            {
                return null;
            }

            GroupModel model = new GroupModel();

            model.Id = response.value[0].id;
            model.DisplayName = response.value[0].displayName;
            model.Description = response.value[0].description;

            return model;
        }
    }
}

using System.Collections.Generic;
using TryIT.MicrosoftGraphService.Model;
using static TryIT.MicrosoftGraphService.ApiModel.GroupMemberResponse;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class GroupMemberResponseExt
    {
        public static List<GroupMemberModel> ToGroupModels(this GetGroupMemberResponse response)
        {
            if (response == null || response.value == null || response.value.Count == 0)
            {
                return null;
            }

            List<GroupMemberModel> models = new List<GroupMemberModel>();

            response.value.ForEach(x =>
            {
                models.Add(new GroupMemberModel
                {
                    Id = x.id,
                    DisplayName = x.displayName,
                    Mail = x.mail,
                    UserPrincipalName = x.userPrincipalName
                });
            });

            return models;
        }
    }
}

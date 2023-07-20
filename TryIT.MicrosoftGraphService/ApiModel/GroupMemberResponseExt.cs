using TryIT.MicrosoftGraphService.Model;
using static TryIT.MicrosoftGraphService.ApiModel.GroupMemberResponse;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class GroupMemberResponseExt
    {
        public static GroupMemberModel ToGroupModel(this Member response)
        {
            if (response == null)
            {
                return null;
            }

            return new GroupMemberModel
            {
                Id = response.id,
                DisplayName = response.displayName,
                Mail = response.mail,
                UserPrincipalName = response.userPrincipalName
            };
        }
    }
}

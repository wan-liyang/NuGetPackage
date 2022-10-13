using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class TeamResponseExt
    {
        public static TeamModel.Team ToTeamModel_Team(this TeamResponse.Team response)
        {
            TeamModel.Team model = new TeamModel.Team();
            if (response != null)
            {
                model.Id = response.id;
                model.DisplayName = response.displayName;
            }
            return model;
        }
        public static TeamModel.Member ToTeamModel_Member(this TeamResponse.Member response)
        {
            TeamModel.Member model = new TeamModel.Member();
            if (response != null)
            {
                model.MembershipId = response.id;
                model.DisplayName = response.displayName;
                model.UserId = response.userId;
                model.UserEmail = response.email;
            }
            return model;
        }
    }
}

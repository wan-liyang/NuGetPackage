namespace MicrosoftGraphService.Model
{
    public class TeamModel
    {
        public class Team
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }

        public class Member
        {
            public string MembershipId { get; set; }
            public string DisplayName { get; set; }
            public string UserId { get; set; }
            public string UserEmail { get; set; }
        }
    }
}

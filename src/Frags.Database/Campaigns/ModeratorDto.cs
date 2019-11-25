using Frags.Database.Characters;

namespace Frags.Database.Campaigns
{
    public class ModeratorDto
    {
        public UserDto User { get; set; }
        public int UserId { get; set; }
        public CampaignDto Campaign { get; set; }
        public int CampaignId { get; set; }
    }
}
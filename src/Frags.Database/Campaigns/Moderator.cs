using Frags.Core.Campaigns;
using Frags.Database.Characters;

namespace Frags.Database.Campaigns
{
    public class Moderator
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CampaignId { get; set; }
        public CampaignDto Campaign { get; set; }
    }
}
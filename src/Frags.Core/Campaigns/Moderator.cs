using Frags.Core.Common;

namespace Frags.Core.Campaigns
{
    public class Moderator
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public Campaign Campaign { get; set; }
        public int CampaignId { get; set; }
    }
}
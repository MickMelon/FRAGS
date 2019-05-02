using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;

namespace Frags.Core.DataAccess
{
    public class MockCampaignProvider : ICampaignProvider
    {
        private List<Campaign> _campaigns = new List<Campaign>();
        private int _id = 1;
        
        public Task<Campaign> CreateCampaignAsync(ulong ownerId, string name)
        {
            var campaign = new Campaign(ownerId, name);
            campaign.Id = _id++;
            _campaigns.Add(campaign);
            return Task.FromResult(campaign);
        }

        public Task<Campaign> GetCampaignAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id));
        }

        public Task UpdateCampaignAsync(Campaign campaign)
        {
            _campaigns[_campaigns.FindIndex(x => x.Id == campaign.Id)] = campaign;
            return Task.CompletedTask;
        }
    }
}
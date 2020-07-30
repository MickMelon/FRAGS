using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    public class MockCampaignController : ICampaignController
    {
        private List<Campaign> _campaigns = new List<Campaign>();

        public Task<string> AddCampaignChannelAsync(string campaignName, ulong channelId)
        {
            var campaign = _campaigns.Find(x => x.Name == campaignName);

            campaign.Channels.Add(new Channel(channelId, campaign));

            return Task.FromResult("done");
        }

        public Task<string> ConfigureCampaignAsync(ulong callerId, ulong channelId, string property, object value)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CreateCampaignAsync(ulong callerId, string name)
        {
            _campaigns.Add(new Campaign(new User(callerId), name));
            return Task.FromResult("done");
        }

        public Task<string> GetCampaignInfoAsync(ulong channelId)
        {
            foreach (var campaign in _campaigns)
            {
                if (campaign.Channels.Any(x => x.Id == channelId))
                {
                    return Task.FromResult(campaign.Name);
                }
            }

            return Task.FromResult<string>("campaign not found");
        }

        public Task<string> GetCampaignInfoAsync(string name)
        {
            var campaign = _campaigns.Find(x => x.Name.EqualsIgnoreCase(name));
            if (campaign == null) return Task.FromResult("Campaign not found!");

            return Task.FromResult(campaign.Name);
        }
    }
}
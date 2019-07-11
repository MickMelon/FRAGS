using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

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

        public Task<bool> DeleteCampaignAsync(Campaign campaign)
        {
            return Task.FromResult(_campaigns.Remove(campaign));
        }

        public Task<Campaign> GetCampaignAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id));
        }

        public Task<ICollection<ulong>> GetChannelsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).Channels);
        }

        public Task<ICollection<Character>> GetCharactersAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).Characters);
        }

        public Task<ICollection<Effect>> GetEffectsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).Effects);
        }

        public Task<ICollection<ulong>> GetModeratorsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).ModeratorUserIdentifiers);
        }

        public Task<RollOptions> GetRollOptionsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).RollOptions);
        }

        public Task<StatisticOptions> GetStatisticOptionsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).StatisticOptions);
        }

        public Task<ICollection<Statistic>> GetStatisticsAsync(int id)
        {
            return Task.FromResult(_campaigns.FirstOrDefault(x => x.Id == id).Statistics);
        }

        public Task UpdateCampaignAsync(Campaign campaign)
        {
            _campaigns[_campaigns.FindIndex(x => x.Id == campaign.Id)] = campaign;
            return Task.CompletedTask;
        }
    }
}
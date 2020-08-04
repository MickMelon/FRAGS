using System;
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
    public class MockCampaignProvider : ICampaignProvider
    {
        private List<Campaign> _campaigns = new List<Campaign>();

        private readonly IUserProvider _userProvider;

        public MockCampaignProvider(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public async Task CreateCampaignAsync(ulong userIdentifier, string name)
        {
            User user = await _userProvider.GetUserAsync(userIdentifier);

            if (user == null)
                user = await _userProvider.CreateUserAsync(userIdentifier);

            _campaigns.Add(new Campaign(user, name));
        }

        public Task DeleteCampaignAsync(string campaignName)
        {
            _campaigns.RemoveAll(x => x.Name.EqualsIgnoreCase(campaignName));
            return Task.CompletedTask;
        }

        public Task<Campaign> GetCampaignAsync(string campaignName)
        {
            return Task.FromResult(_campaigns.Find(x => x.Name.EqualsIgnoreCase(campaignName)));
        }

        public Task<Campaign> GetCampaignAsync(ulong channelId)
        {
            foreach (var campaign in _campaigns)
            {
                if (campaign.Channels != null && campaign.Channels.Any(x => x.Id == channelId))
                    return Task.FromResult(campaign);
            }

            return null;
        }

        public Task<List<Channel>> GetChannelsAsync(Campaign campaign)
        {
            return Task.FromResult(campaign.Channels);
        }

        public Task<List<Character>> GetCharactersAsync(Campaign campaign)
        {
            return Task.FromResult(campaign.Characters);
        }

        public Task<List<Moderator>> GetModeratorsAsync(Campaign campaign)
        {
            return Task.FromResult(campaign.ModeratedCampaigns);
        }

        public Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign)
        {
            return Task.FromResult(campaign.StatisticOptions);
        }

        public Task UpdateCampaignAsync(Campaign campaign)
        {
            _campaigns[_campaigns.FindIndex(x => x.Id == campaign.Id)] = campaign;
            return Task.CompletedTask;
        }

        public Task ConfigureCampaignAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            Campaign campaign = GetCampaignAsync(channelId).Result;
            StatisticOptions stats = GetStatisticOptionsAsync(campaign).Result;
            if (stats == null) stats = new StatisticOptions();
            var propertyInfo = stats.GetType().GetProperty(propName);
            var propertyType = propertyInfo.PropertyType;
            value = Convert.ChangeType(value, propertyType);
            propertyInfo.SetValue(stats, value);

            campaign.StatisticOptions = stats;
            return Task.CompletedTask;
        }

        public Task ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            Campaign campaign = GetCampaignAsync(channelId).Result;
            campaign.Characters.Add(new Character(campaign.Owner, campaign.Name));
            return Task.CompletedTask;
        }

        public Task AddChannelAsync(string campaignName, ulong channelId)
        {
            Campaign camp = _campaigns.Find(x => x.Name.EqualsIgnoreCase(campaignName));
            if (camp.Channels == null) camp.Channels = new List<Channel>();

            camp.Channels.Add(new Channel(channelId, camp));
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
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

        public Task DeleteCampaignAsync(Campaign campaign)
        {
            _campaigns.Remove(campaign);
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

            return Task.FromResult<Campaign>(null);
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

        public Task RenameCampaignAsync(ulong callerId, string newName, ulong channelId)
        {
            if (GetCampaignAsync(newName).Result != null)
                return Task.FromException(new CampaignException(Messages.CAMP_EXISTING_NAME));

            Campaign campaign = GetCampaignAsync(channelId).Result;
            if (campaign == null) return Task.FromException(new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL));

            // Caller must be owner or present in Moderators
            if (campaign.Owner.UserIdentifier != callerId || (campaign.ModeratedCampaigns != null && !campaign.ModeratedCampaigns.Select(x => x.User).Any(x => x.UserIdentifier == callerId)))
                return Task.FromException(new CampaignException(Messages.CAMP_ACCESS_DENIED));

            campaign.Name = newName;
            UpdateCampaignAsync(campaign);
            return Task.CompletedTask;
        }

        public Task RemoveChannelAsync(ulong channelId)
        {
            foreach (var campaign in _campaigns)
            {
                foreach (var channel in campaign.Channels)
                {
                    if (channel.Id == channelId)
                    {
                        campaign.Channels.Remove(channel);
                    }
                }
            }
            
            return Task.CompletedTask;
        }

        public Task UpdateModeratorsAsync(Campaign campaign, List<Moderator> moderators)
        {
            _campaigns[_campaigns.IndexOf(campaign)].ModeratedCampaigns = moderators;
            return Task.CompletedTask;
        }

        public Task UpdateStatisticOptionsAsync(Campaign campaign, StatisticOptions statisticOptions)
        {
            _campaigns[_campaigns.IndexOf(campaign)].StatisticOptions = statisticOptions;
            return Task.CompletedTask;
        }

        public Task RenameCampaignAsync(Campaign campaign, string newName)
        {
            _campaigns[_campaigns.IndexOf(campaign)].Name = newName;
            return Task.CompletedTask;
        }

        public Task UpdateChannelsAsync(Campaign campaign, List<Channel> channels)
        {
            _campaigns[_campaigns.IndexOf(campaign)].Channels = channels;
            return Task.CompletedTask;
        }

        public Task<bool> HasPermissionAsync(Campaign camp, ulong userIdentifier)
        {
            return Task.FromResult(camp.Owner.UserIdentifier == userIdentifier || (camp.ModeratedCampaigns?.Select(x => x.User.UserIdentifier)?.Contains(userIdentifier) ?? false));
        }

        public Task SetCampaignChannelAsync(Campaign campaign, ulong channelId)
        {
            List<Channel> channels = _campaigns[_campaigns.IndexOf(campaign)].Channels;
            if (channels == null) channels = new List<Channel>();
            
            channels.Add(new Channel(channelId, campaign));
            return Task.CompletedTask;
        }

        public Task<RollOptions> GetRollOptionsAsync(Campaign campaign)
        {
            return Task.FromResult(campaign.RollOptions);
        }

        public Task UpdateRollOptionsAsync(Campaign campaign, RollOptions rollOptions)
        {
            _campaigns[_campaigns.IndexOf(campaign)].RollOptions = rollOptions;
            return Task.CompletedTask;
        }
    }
}
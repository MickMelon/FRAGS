using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class CampaignController
    {
        private readonly ICampaignProvider _provider;
        private readonly IUserProvider _userProvider;
        private readonly GeneralOptions _generalOptions;
        private readonly ICharacterProvider _charProvider;
        private readonly List<IProgressionStrategy> _progStrategies;

        public CampaignController(
            ICampaignProvider provider,
            IUserProvider userProvider,
            GeneralOptions generalOptions,
            ICharacterProvider charProvider,
            List<IProgressionStrategy> progStrategies)
        {
            _provider = provider;
            _userProvider = userProvider;
            _generalOptions = generalOptions;
            _charProvider = charProvider;
            _progStrategies = progStrategies;
        }

        public async Task<IResult> ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            var campaign = await _provider.GetCampaignFromChannelAsync(channelId);
            if (campaign == null) return GenericResult.Failure("Channel not associated with Campaign.");

            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var statOptions = await _provider.GetStatisticOptionsAsync(campaign.Id);
            if (statOptions == null) return GenericResult.Failure("Campaign has not set its StatisticOptions!");

            var strategy = GetProgressionStrategy(statOptions);
            if (strategy == null) return GenericResult.Failure("Campaign has not set its ProgressionStrategy (or it's invalid!)");

            await strategy.ResetCharacter(character);
            character.Campaign = campaign;

            await _charProvider.UpdateCharacterAsync(character);
            return GenericResult.Generic("Character converted.");
        }

        public async Task<IResult> CreateCampaignAsync(ulong callerId, string name)
        {
            var ownedCampaigns = await _provider.GetOwnedCampaignsAsync(callerId);
            if (ownedCampaigns != null && ownedCampaigns.Count >= _generalOptions.CampaignLimit)
                return GenericResult.Failure("Too many campaigns!");

            await _provider.CreateCampaignAsync(callerId, name);
            return GenericResult.Generic("Campaign created.");
        }

        public async Task<IResult> ConfigureCampaignAsync(ulong callerId, ulong channelId)
        {
            var user = await _userProvider.GetUserAsync(callerId);
            if (user == null) return GenericResult.Failure("User does not exist and therefore does not moderate or own any Campaigns.");

            var campaign = await _provider.GetCampaignFromChannelAsync(channelId);
            if (campaign == null) return GenericResult.Failure("Channel not associated with a Campaign.");

            var moderators = await _provider.GetModeratorsAsync(campaign.Id);

            // Caller is a moderator or owner of this campaign
            if (campaign.Owner.Id == user.Id || (moderators != null && moderators.Any(x => x.Id == user.Id)))
            {
                campaign.StatisticOptions = new StatisticOptions
                {
                    ExpEnabledChannels = new ulong[] { channelId },
                    ProgressionStrategy = "Generic"
                };
                return GenericResult.Generic("Done.");
            }
            else
            {
                return GenericResult.Failure("You don't have permission to do that.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelId">ID of the caller's current channel.</param>
        /// <returns>A new Result.</returns>
        public async Task<IResult> AddCampaignChannelAsync(string campaignName, ulong channelId)
        {
            var campaign = await _provider.GetCampaignAsync(campaignName);

            if (campaign == null)
                return GenericResult.Failure("Campaign not found.");

            if (campaign.Channels.Any(x => x.Id == channelId))
                return GenericResult.Failure("Channel already added.");

            campaign.Channels.Add(new Core.Campaigns.Channel(channelId, campaign));
            await _provider.UpdateCampaignAsync(campaign);

            return GenericResult.Generic("Success.");
        }

        public async Task<IResult> GetCampaignInfoAsync(ulong channelId)
        {
            var campaign = await _provider.GetCampaignFromChannelAsync(channelId);

            if (campaign == null)
                return GenericResult.Failure("Campaign not associated with channel.");

            return await GetCampaignInfoAsync(campaign);
        }

        public async Task<IResult> GetCampaignInfoAsync(string campaignName)
        {
            var campaign = await _provider.GetCampaignAsync(campaignName);

            if (campaign == null)
                return GenericResult.Failure("Campaign not found.");

            return await GetCampaignInfoAsync(campaign);
        }

        private async Task<IResult> GetCampaignInfoAsync(Campaign campaign)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(campaign.Name + "\n\n");
            
            var moderators = await _provider.GetModeratorsAsync(campaign.Id);
            sb.Append("**Moderators:** " + String.Join(", ", moderators.Select(x => x.UserIdentifier)) + "\n");

            var channels = await _provider.GetChannelsAsync(campaign.Id);
            sb.Append("**Channels:** " + String.Join(", ", channels.Select(x => x.Id)) + "\n");

            var characters = await _provider.GetCharactersAsync(campaign.Id);
            sb.Append("**Characters:** " + String.Join(", ", characters.Select(x => x.Name)) + "\n");

            return GenericResult.Generic(sb.ToString());
        }

        private IProgressionStrategy GetProgressionStrategy(StatisticOptions options) =>
            _progStrategies.Find(x => x.GetType().Name.ContainsIgnoreCase(options.ProgressionStrategy));
    }
}
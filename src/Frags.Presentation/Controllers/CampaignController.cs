using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class CampaignController
    {
        private readonly IUserProvider _userProvider;

        private readonly ICharacterProvider _charProvider;

        private readonly ICampaignProvider _campProvider;

        private readonly IStatisticProvider _statProvider;
        

        public CampaignController(IUserProvider userProvider,
        ICharacterProvider charProvider,
        ICampaignProvider campProvider,
        IStatisticProvider statProvider)
        {
            _userProvider = userProvider;
            _charProvider = charProvider;
            _campProvider = campProvider;
            _statProvider = statProvider;
        }

        public async Task<IResult> RenameCampaignAsync(ulong callerId, ulong channelId, string newName)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            if (await _campProvider.GetCampaignAsync(newName) != null)
                return CampaignResult.NameAlreadyExists();

            await _campProvider.RenameCampaignAsync(campaign, newName);
            return CampaignResult.NameChanged();
        }

        public async Task<IResult> AddCampaignChannelAsync(string campaignName, ulong channelId)
        {
            Campaign campaignToSet = await _campProvider.GetCampaignAsync(campaignName);
            if (campaignToSet == null) return CampaignResult.NotFoundByName();

            // We should warn the user if there is already a Campaign associated with this channel.
            Campaign previouslySet = await _campProvider.GetCampaignAsync(channelId);
            if (previouslySet == null)
            {
                await _campProvider.SetCampaignChannelAsync(campaignToSet, channelId);
                return CampaignResult.ChannelAdded();
            }
            else
            {
                return CampaignResult.ChannelAlreadyPresent();
            }
        }

        public async Task<IResult> RemoveCampaignChannelAsync(ulong channelId)
        {
            if (await _campProvider.GetCampaignAsync(channelId) == null)
                return CampaignResult.NotFoundByChannel();

            await _campProvider.SetCampaignChannelAsync(null, channelId);
            return CampaignResult.ChannelRemoved();
        }

        public async Task<IResult> ConfigureStatisticOptionsAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            // Caller is not a moderator or owner of this campaign
            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            StatisticOptions statOptions = await _campProvider.GetStatisticOptionsAsync(campaign);
            if (statOptions == null) statOptions = new StatisticOptions();

            if (propName == null || propName == nameof(statOptions.Id) || propName == nameof(statOptions.ExpEnabledChannels)) 
                    return CampaignResult.InvalidProperty();

            try
            {
                SetOptionsProperty(propName, value, statOptions);
                await _campProvider.UpdateStatisticOptionsAsync(campaign, statOptions);
                return CampaignResult.PropertyChanged();
            }
            catch (System.Exception e)
            {
                return CampaignResult.InvalidPropertyValue(e.Message);
            }
        }

        public async Task<IResult> ConfigureRollOptionsAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            // Caller is not a moderator or owner of this campaign
            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            RollOptions rollOptions = await _campProvider.GetRollOptionsAsync(campaign);
            if (rollOptions == null) rollOptions = new RollOptions();

            if (propName == null || propName == nameof(rollOptions.Id))
                    return CampaignResult.InvalidProperty();

            try
            {
                SetOptionsProperty(propName, value, rollOptions);
                await _campProvider.UpdateRollOptionsAsync(campaign, rollOptions);
                return CampaignResult.PropertyChanged();
            }
            catch (System.Exception e)
            {
                return CampaignResult.InvalidPropertyValue(e.Message);
            }
        }

        private void SetOptionsProperty(string propName, object value, object toConfigure)
        {
            // Try to match propName to a property in our given object to configure
            var propertyInfo = toConfigure.GetType().GetProperty(propName);

            // Try to convert our given value (probably a string or int) to the same type as the property
            var propertyType = propertyInfo.PropertyType;
            value = Convert.ChangeType(value, propertyType);

            propertyInfo.SetValue(toConfigure, value);
        }

        public async Task<IResult> ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            Character character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            IProgressionStrategy strategy = await _campProvider.GetProgressionStrategy(campaign);
            if (strategy == null) return CampaignResult.InvalidProgressionStrategy();

            character.Campaign = campaign;
            await strategy.ResetCharacter(character);
            
            character.CampaignId = campaign.Id;

            await _charProvider.UpdateCharacterAsync(character);
            return CampaignResult.CharacterConverted();
        }

        public async Task<IResult> CreateCampaignAsync(ulong userIdentifier, string name)
        {
            if (await _campProvider.GetCampaignAsync(name) != null)
                return CampaignResult.NameAlreadyExists();

            await _campProvider.CreateCampaignAsync(userIdentifier, name);

            return CampaignResult.CampaignCreated();
        }

        public async Task<IResult> GetCampaignInfoAsync(string campaignName) =>
            await GetCampaignInfo(await _campProvider.GetCampaignAsync(campaignName));

        public async Task<IResult> GetCampaignInfoAsync(ulong channelId) =>
            await GetCampaignInfo(await _campProvider.GetCampaignAsync(channelId));

        private async Task<IResult> GetCampaignInfo(Campaign campaign)
        {
            if (campaign == null) return GenericResult.Failure("Campaign not found!");

            List<Channel> channels = await _campProvider.GetChannelsAsync(campaign);
            List<Character> characters = await _campProvider.GetCharactersAsync(campaign);
            StatisticOptions statOptions = await _campProvider.GetStatisticOptionsAsync(campaign);
            RollOptions rollOptions = await _campProvider.GetRollOptionsAsync(campaign);
            IEnumerable<Statistic> statistics = await _statProvider.GetAllStatisticsFromCampaignAsync(campaign);

            return CampaignResult.Show(campaign, channels, characters, statOptions, rollOptions, statistics);
        }
    }
}
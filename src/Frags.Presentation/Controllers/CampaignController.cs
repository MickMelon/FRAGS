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
        

        public CampaignController(IUserProvider userProvider,
        ICharacterProvider charProvider,
        ICampaignProvider campProvider)
        {
            _userProvider = userProvider;
            _charProvider = charProvider;
            _campProvider = campProvider;
        }

        public async Task<IResult> RenameCampaignAsync(ulong callerId, string newName, ulong channelId)
        {
            try
            {
                await _campProvider.RenameCampaignAsync(callerId, newName, channelId);
            }
            catch (CampaignException e)
            {
                return GenericResult.Failure(e.Message);
            }

            return CampaignResult.NameChanged();
        }

        public async Task<IResult> AddCampaignChannelAsync(string campaignName, ulong channelId)
        {
            try
            {
                await _campProvider.AddChannelAsync(campaignName, channelId);   
            }
            catch (CampaignException e)
            {
                return GenericResult.Failure(e.Message);
            }

            return CampaignResult.ChannelAdded();
        }

        public async Task<IResult> ConfigureCampaignAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            try
            {
                await _campProvider.ConfigureCampaignAsync(callerId, channelId, propName, value);
            }
            catch (CampaignException e)
            {
                return GenericResult.Failure(e.Message);
            }
            
            return CampaignResult.PropertyChanged();
        }

        public async Task<IResult> ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            try
            {
                await _campProvider.ConvertCharacterAsync(callerId, channelId);    
            }
            catch (CampaignException e)
            {
                return GenericResult.Failure(e.Message);
            }
            
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

            return CampaignResult.Show(campaign, channels, characters, statOptions);
        }
    }
}
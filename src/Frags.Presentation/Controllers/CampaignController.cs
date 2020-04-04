using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Common;
using Frags.Core.DataAccess;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class CampaignController
    {
        private readonly ICampaignProvider _provider;

        public CampaignController(ICampaignProvider provider)
        {
            _provider = provider;
        }

        public async Task<IResult> CreateCampaignAsync(ulong callerId, string name)
        {
            await _provider.CreateCampaignAsync(callerId, name);
            return GenericResult.Generic("Campaign created.");
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
            sb.Append("**Moderators:** " + String.Join(", ", moderators.Select(x => x.UserIdentifier)) + "\n\n");

            var channels = await _provider.GetChannelsAsync(campaign.Id);
            sb.Append("**Channels:** " + String.Join(", ", channels.Select(x => x.Id)) + "\n\n");

            var characters = await _provider.GetCharactersAsync(campaign.Id);
            sb.Append("**Characters:** " + String.Join(", ", characters.Select(x => x.Name)) + "\n\n");

            return GenericResult.Generic(sb.ToString());
        }
    }
}
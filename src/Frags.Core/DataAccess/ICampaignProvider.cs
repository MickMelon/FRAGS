using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    public interface ICampaignProvider
    {
        Task<Campaign> GetCampaignAsync(string campaignName);
        Task<Campaign> GetCampaignAsync(ulong channelId);
        
        Task<List<Moderator>> GetModeratorsAsync(Campaign campaign);
        Task UpdateModeratorsAsync(Campaign campaign, List<Moderator> moderators);

        Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign);
        Task UpdateStatisticOptionsAsync(Campaign campaign, StatisticOptions statisticOptions);

        Task RenameCampaignAsync(Campaign campaign, string newName);

        Task<List<Channel>> GetChannelsAsync(Campaign campaign);
        Task UpdateChannelsAsync(Campaign campaign, List<Channel> channels);

        Task<List<Character>> GetCharactersAsync(Campaign campaign);
        Task<bool> HasPermissionAsync(Campaign campaign, ulong userIdentifier);
        Task CreateCampaignAsync(ulong userIdentifier, string name);
        Task DeleteCampaignAsync(Campaign campaign);
        Task SetCampaignChannelAsync(Campaign campaign, ulong channelId);
    }
}
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
        Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign);
        Task UpdateCampaignAsync(Campaign campaign);
        Task<List<Channel>> GetChannelsAsync(Campaign campaign);
        Task<List<Character>> GetCharactersAsync(Campaign campaign);
        Task CreateCampaignAsync(ulong userIdentifier, string name);
        Task DeleteCampaignAsync(string campaignName);
        Task ConfigureCampaignAsync(ulong callerId, ulong channelId, string propName, object value);
        Task ConvertCharacterAsync(ulong callerId, ulong channelId);
        Task AddChannelAsync(string campaignName, ulong channelId);
    }
}
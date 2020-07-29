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
    public interface ICampaignController
    {
        Task<string> ConvertCharacterAsync(ulong callerId, ulong channelId);
        Task<string> CreateCampaignAsync(ulong callerId, string name);
        Task<string> AddCampaignChannelAsync(string campaignName, ulong channelId);
        Task<string> GetCampaignInfoAsync(ulong channelId);
        Task<string> GetCampaignInfoAsync(string name);
        Task<string> ConfigureCampaignAsync(ulong callerId, ulong channelId, string property, object value);
    }
}
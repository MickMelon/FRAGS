using System;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.DataAccess;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class CampaignStatisticController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _charProvider;

        /// <summary>
        /// Used to interact with the statistic database.
        /// </summary>
        private readonly IStatisticProvider _statProvider;

        private readonly ICampaignProvider _campProvider;

        public CampaignStatisticController(ICharacterProvider charProvider, IStatisticProvider statProvider, ICampaignProvider campProvider)
        {
            _charProvider = charProvider;
            _statProvider = statProvider;
            _campProvider = campProvider;
        }

        public async Task<IResult> AddCampaignAliasAsync(string statName, string alias, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();

            var stat = await _statProvider.GetStatisticFromCampaignAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            if (await _statProvider.GetStatisticFromCampaignAsync(alias, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            stat.Aliases += alias + "/";
            await _statProvider.UpdateStatisticAsync(stat);

            return StatisticResult.StatisticUpdatedSucessfully();
        }

        public async Task<IResult> ClearCampaignAliasesAsync(string statName, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            var stat = await _statProvider.GetStatisticFromCampaignAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Aliases = stat.Name + "/";
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        public async Task<IResult> DeleteCampaignStatisticAsync(string statName, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            var stat = await _statProvider.GetStatisticFromCampaignAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            await _statProvider.DeleteStatisticAsync(stat);
            return StatisticResult.StatisticDeletedSuccessfully();
        }

        public async Task<IResult> RenameCampaignStatisticAsync(string statName, string newName, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);
            if (campaign == null) return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            var stat = await _statProvider.GetStatisticFromCampaignAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Name = newName;
            stat.Aliases = stat.Name + "/";
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        /// <summary>
        /// Creates a new Attribute associated with a Campaign in the database.
        /// </summary>
        /// <param name="statName">The name for the new attribute.</param>
        /// <param name="callerId">The id of the user executing the command.</param>
        /// <param name="channelId">The id of the channel where the command was executed from.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateCampaignAttributeAsync(string statName, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);

            if (campaign == null)
                return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            if (await _statProvider.GetStatisticFromCampaignAsync(statName, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateAttributeAsync(statName, campaign);

            if (result == null)
                return StatisticResult.StatisticCreationFailed();
                
            return StatisticResult.StatisticCreatedSuccessfully();
        }

        /// <summary>
        /// Creates a new Skill associated with a Campaign in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// <param name="attribName">The name of the attribute to go with the skill. Must exist in the database beforehand.</param>
        /// <param name="callerId">The id of the user executing the command.</param>
        /// <param name="channelId">The id of the channel where the command was executed from.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateCampaignSkillAsync(string statName, string attribName, ulong callerId, ulong channelId)
        {
            Campaign campaign = await _campProvider.GetCampaignAsync(channelId);

            if (campaign == null)
                return CampaignResult.NotFoundByChannel();

            if (!await _campProvider.HasPermissionAsync(campaign, callerId))
                return CampaignResult.AccessDenied();

            if (await _statProvider.GetStatisticFromCampaignAsync(statName, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateSkillAsync(statName, attribName, campaign);

            if (result == null)
                return StatisticResult.StatisticCreationFailed();

            return StatisticResult.StatisticCreatedSuccessfully();
        }
    }
}
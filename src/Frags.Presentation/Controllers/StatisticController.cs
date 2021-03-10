using System;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.DataAccess;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class StatisticController
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

        public StatisticController(ICharacterProvider charProvider, IStatisticProvider statProvider, ICampaignProvider campProvider)
        {
            _charProvider = charProvider;
            _statProvider = statProvider;
            _campProvider = campProvider;
        }

        public async Task<IResult> AddAliasAsync(string statName, string alias) =>
            await AddAliasAsync(statName, alias, 0, 0, useCampaigns: false);

        public async Task<IResult> AddCampaignAliasAsync(string statName, string alias, ulong callerId, ulong channelId) =>
            await AddAliasAsync(statName, alias, callerId, channelId, useCampaigns: true);

        private async Task<IResult> AddAliasAsync(string statName, string alias, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            if (await _statProvider.GetStatisticAsync(alias, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            stat.Aliases += alias + "/";
            await _statProvider.UpdateStatisticAsync(stat);

            return StatisticResult.StatisticUpdatedSucessfully();
        }

        public async Task<IResult> ClearAliasesAsync(string statName) =>
            await ClearAliasesAsync(statName, 0, 0, useCampaigns: false);

        public async Task<IResult> ClearCampaignAliasesAsync(string statName, ulong callerId, ulong channelId) =>
            await ClearAliasesAsync(statName, callerId, channelId, useCampaigns: true);

        private async Task<IResult> ClearAliasesAsync(string statName, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Aliases = stat.Name + "/";
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        public async Task<IResult> OrderStatisticAsync(string statName, int order) =>
            await OrderStatisticAsync(statName, order, 0, 0, useCampaigns: false);

        public async Task<IResult> OrderCampaignStatisticAsync(string statName, int order, ulong callerId, ulong channelId) =>
            await OrderStatisticAsync(statName, order, callerId, channelId, useCampaigns: true);

        private async Task<IResult> OrderStatisticAsync(string statName, int order, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Order = order;
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        public async Task<IResult> DeleteStatisticAsync(string statName) =>
            await DeleteStatisticAsync(statName, 0, 0, useCampaigns: false);

        public async Task<IResult> DeleteCampaignStatisticAsync(string statName, ulong callerId, ulong channelId) =>
            await DeleteStatisticAsync(statName, callerId, channelId, useCampaigns: true);

        private async Task<IResult> DeleteStatisticAsync(string statName, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            await _statProvider.DeleteStatisticAsync(stat);
            return StatisticResult.StatisticDeletedSuccessfully();
        }

        public async Task<IResult> RenameStatisticAsync(string statName, string newName) =>
            await RenameStatisticAsync(statName, newName, 0, 0, useCampaigns: false);

        public async Task<IResult> RenameCampaignStatisticAsync(string statName, string newName, ulong callerId, ulong channelId) =>
            await RenameStatisticAsync(statName, newName, callerId, channelId, useCampaigns: true);

        private async Task<IResult> RenameStatisticAsync(string statName, string newName, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Name = newName;
            stat.Aliases = stat.Name + "/";
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        /// <summary>
        /// Creates a new Attribute that is NOT associated with a Campaign.
        /// </summary>
        /// <param name="statName">The name for the new attribute.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateAttributeAsync(string statName) =>
            await CreateAttributeAsync(statName, 0, 0, useCampaigns: false);

        /// <summary>
        /// Creates a new Attribute, associated with a Campaign in the database.
        /// </summary>
        /// <param name="statName">The name for the new attribute.</param>
        /// <param name="callerId">The id of the user executing the command.</param>
        /// <param name="channelId">The id of the channel where the command was executed from.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateCampaignAttributeAsync(string statName, ulong callerId, ulong channelId) =>
            await CreateAttributeAsync(statName, callerId, channelId, useCampaigns: true);

        private async Task<IResult> CreateAttributeAsync(string statName, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            if (await _statProvider.GetStatisticAsync(statName, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateAttributeAsync(statName, campaign);

            if (result == null)
                return StatisticResult.StatisticCreationFailed();
                
            return StatisticResult.StatisticCreatedSuccessfully();
        }

        /// <summary>
        /// Creates a new Skill, associated with a Campaign in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// <param name="attribName">The name of the attribute to go with the skill. Must exist in the database beforehand.</param>
        /// <param name="callerId">The id of the user executing the command.</param>
        /// <param name="channelId">The id of the channel where the command was executed from.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateCampaignSkillAsync(string statName, string attribName, ulong callerId, ulong channelId) =>
            await CreateSkillAsync(statName, attribName, callerId, channelId, useCampaigns: true);

        /// <summary>
        /// Creates a new Skill, NOT associated with a Campaign in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// <param name="attribName">The name of the attribute to go with the skill. Must exist in the database beforehand.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateSkillAsync(string statName, string attribName) =>
            await CreateSkillAsync(statName, attribName, 0, 0, useCampaigns: false);

        private async Task<IResult> CreateSkillAsync(string statName, string attribName, ulong callerId, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            if (await _statProvider.GetStatisticAsync(statName, campaign) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateSkillAsync(statName, attribName, campaign);

            if (result == null)
                return StatisticResult.StatisticCreationFailed();

            return StatisticResult.StatisticCreatedSuccessfully();
        }
    }
}
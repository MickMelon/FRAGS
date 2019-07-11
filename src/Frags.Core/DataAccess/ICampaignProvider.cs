using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    public interface ICampaignProvider
    {
        /// <summary>
        /// Adds a new campaign to the database.
        /// </summary>
        /// <param name="name">Campaign's name.</param>
        /// <returns>The added campaign if successful, null if not.</returns>
        Task<Campaign> CreateCampaignAsync(ulong ownerId, string name);

        /// <summary>
        /// Gets the Campaign with the matching id from the database.
        /// The returned Campaign object may not have navigation properties (Collections, Options, and certain complex types) included with it.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>The matching campaign or null if none.</returns>
        Task<Campaign> GetCampaignAsync(int id);

        /// <summary>
        /// Gets the Moderators of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>A collection of matching moderators or an empty collection if none found.</returns>
        Task<ICollection<ulong>> GetModeratorsAsync(int id);

        /// <summary>
        /// Gets the Channels of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>A collection of matching channels or an empty collection if none found.</returns>
        Task<ICollection<ulong>> GetChannelsAsync(int id);

        /// <summary>
        /// Gets the Characters of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>A collection of matching characters or an empty collection if none found.</returns>
        Task<ICollection<Character>> GetCharactersAsync(int id);

        /// <summary>
        /// Gets the Effects of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>A collection of matching effects or an empty collection if none found.</returns>
        Task<ICollection<Effect>> GetEffectsAsync(int id);

        /// <summary>
        /// Gets the Statistics of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>A collection of matching statistics or an empty collection if none found.</returns>
        Task<ICollection<Statistic>> GetStatisticsAsync(int id);

        /// <summary>
        /// Gets the RollOptions of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>The matching RollOptions for the campaign or null if none.</returns>
        Task<RollOptions> GetRollOptionsAsync(int id);

        /// <summary>
        /// Gets the StatisticOptions of a Campaign from the database.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>The matching StatisticOptions for the campaign or null if none.</returns>
        Task<StatisticOptions> GetStatisticOptionsAsync(int id);

        /// <summary>
        /// Updates a Campaign in the database.
        /// </summary>
        /// <param name="Campaign">The Campaign to be saved.</param>
        Task UpdateCampaignAsync(Campaign Campaign);

        /// <summary>
        /// Deletes a Campaign from the database.
        /// </summary>
        /// <param name="campaign">The Campaign to delete.</param>
        /// <returns>True if the campaign was deleted, false otherwise.</returns>
        Task<bool> DeleteCampaignAsync(Campaign campaign);
    }
}
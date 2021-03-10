using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    /// <summary>
    /// Used for communicating with the characters in the database.
    /// </summary>
    public interface IStatisticProvider
    {
        /// <summary>
        /// Adds a new attribute to the database.
        /// Statistic names should be unique.
        /// </summary>
        /// <param name="name">Attributes's name.</param>
        /// <param name="campaign">The optional campaign to associate this Statistic with.</param>
        /// <returns>The added attribute if successful, null if not.</returns>
        Task<Attribute> CreateAttributeAsync(string name, Campaign campaign);

        /// <summary>
        /// Adds a new skill to the database.
        /// Statistic names should be unique.
        /// </summary>
        /// <param name="name">Skill's name.</param>
        /// <param name="attribName">Attributes's name.</param>
        /// <param name="campaign">The optional campaign to associate this Statistic with.</param>
        /// <returns>The added skill if successful, null if not.</returns>
        Task<Skill> CreateSkillAsync(string name, string attribName, Campaign campaign);

        /// <summary>
        /// Deletes a statistic from the database. 
        /// This method should also remove any and all StatisticMapping's that reference it.
        /// </summary>
        /// <param name="statistic">The statistic to delete.</param>
        Task DeleteStatisticAsync(Statistic statistic);

        /// <summary>
        /// Gets the statistic with the matching ID.
        /// This method should work even if the effect has a Campaign associated with it (or doesn't.)
        /// </summary>
        /// <param name="name">Statistics's name or alias.</param>
        /// <returns>The matching statistic or null if none.</returns>
        Task<Statistic> GetStatisticAsync(int id);

        /// <summary>
        /// Gets the statistic with the matching name or one of its aliases.
        /// </summary>
        /// <param name="name">Statistics's name or alias.</param>
        /// <param name="campaign">The optional Campaign to search from. Must be specified if the Statistic is associated with a Campaign, otherwise returns null.</param>
        /// <returns>The matching statistic or null if none.</returns>
        Task<Statistic> GetStatisticAsync(string name, Campaign campaign);

        /// <summary>
        /// Gets every statistic currently in use.
        /// </summary>
        /// <returns>An Enumerable of statistics currently in use.</returns>
        Task<IEnumerable<Statistic>> GetAllStatisticsAsync(bool includeCampaignStatistics = false);

        /// <summary>
        /// Gets every statistic currently in use in the specified Campaign.
        /// </summary>
        /// <returns>An Enumerable of statistics currently in use.</returns>
        Task<IEnumerable<Statistic>> GetAllStatisticsFromCampaignAsync(Campaign campaign);

        /// <summary>
        /// Updates a statistic in the database.
        /// </summary>
        /// <param name="statistic">The statistic to be saved.</param>
        Task UpdateStatisticAsync(Statistic statistic);
    }
}
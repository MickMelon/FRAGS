using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>The added attribute if successful, null if not.</returns>
        Task<Attribute> CreateAttributeAsync(string name);

        /// <summary>
        /// Adds a new skill to the database.
        /// Statistic names should be unique.
        /// </summary>
        /// <param name="name">Skill's name.</param>
        /// <param name="attribName">Attributes's name.</param>
        /// <returns>The added skill if successful, null if not.</returns>
        Task<Skill> CreateSkillAsync(string name, string attribName);

        /// <summary>
        /// Gets the statistic with the matching name.
        /// </summary>
        /// <param name="name">Statistics's name.</param>
        /// <returns>The matching statistic or null if none.</returns>
        Task<Statistic> GetStatisticAsync(string name);

        /// <summary>
        /// Gets every statistic currently in use.
        /// </summary>
        /// <returns>An Enumerable of statistics currently in use.</returns>
        Task<IEnumerable<Statistic>> GetAllStatisticsAsync();

        /// <summary>
        /// Updates a statistic in the database.
        /// </summary>
        /// <param name="statistic">The statistic to be saved.</param>
        Task UpdateStatisticAsync(Statistic statistic);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;

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
        /// Gets the Campaign with the matching id.
        /// </summary>
        /// <param name="id">Campaigns's id.</param>
        /// <returns>The matching campaign or null if none.</returns>
        Task<Campaign> GetCampaignAsync(int id);

        /// <summary>
        /// Updates a Campaign in the database.
        /// </summary>
        /// <param name="Campaign">The Campaign to be saved.</param>
        Task UpdateCampaignAsync(Campaign Campaign);
    }
}
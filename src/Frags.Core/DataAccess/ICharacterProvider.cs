using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;

namespace Frags.Core.DataAccess
{
    /// <summary>
    /// Used for communicating with the characters in the database.
    /// </summary>
    public interface ICharacterProvider
    {
        /// <summary>
        /// Adds a new character to the database.
        /// </summary>
        /// <param name="userIdentifier">The user identifier.</param>
        /// <param name="name">Character's name.</param>
        /// <returns>Whether the character was added successfully.</returns>
        Task<bool> CreateCharacterAsync(ulong discordId, string name);

        /// <summary>
        /// Gets the active character for the user.
        /// </summary>
        /// <param name="userIdentifier">The user identifier.</param>
        /// <returns>Active character or null if none.</returns>
        Task<Character> GetActiveCharacterAsync(ulong userIdentifier);

        /// <summary>
        /// Gets all the characters for the user.
        /// </summary>
        /// <param name="userIdentifier">The user identifier.</param>
        /// <returns>A list containing all the user's characters.</returns>
        Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier);

        /// <summary>
        /// Gets all the characters from the given campaign.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve characters from.</param>
        /// <returns>A list containing all the campaign's characters.</returns>
        Task<List<Character>> GetAllCampaignCharactersAsync(Campaign campaign);

        /// <summary>
        /// Saves a character in the database.
        /// </summary>
        /// <param name="character">The character to be saved.</param>
        Task UpdateCharacterAsync(Character character);

        /// <summary>
        /// Deletes a character from the database.
        /// </summary>
        /// <param name="character">The character to be deleted.</param>
        Task DeleteCharacterAsync(Character character);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <param name="discordId">Discord ID.</param>
        /// <param name="name">Character's name.</param>
        /// <returns>Whether the character was added successfully.</returns>
        Task<Character> CreateCharacterAsync(ulong discordId, string name);

        /// <summary>
        /// Gets the active character for the user.
        /// </summary>
        /// <param name="discordId">Discord ID</param>
        /// <returns>Active character or null if none.</returns>
        Task<Character> GetActiveCharacterAsync(ulong discordId);

        /// <summary>
        /// Gets all the characters for the user.
        /// </summary>
        /// <param name="discordId">Discord ID</param>
        /// <returns>A list containing all the user's characters.</returns>
        Task<List<Character>> GetAllCharactersAsync(ulong discordId);

        /// <summary>
        /// Saves a character in the database.
        /// </summary>
        /// <param name="character">The character to be saved.</param>
        Task UpdateCharacterAsync(Character character);
    }
}
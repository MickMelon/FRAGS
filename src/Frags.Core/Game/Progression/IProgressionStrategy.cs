using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Progression
{
    public interface IProgressionStrategy
    {
        Task<bool> AddExperience(Character character, int amount);

        /// <summary>
        /// Adds experience to a character depending on the message and what channel it was sent from.
        /// </summary>
        /// <returns>True if adding experience causes the character to level up; false otherwise.</returns>
        Task<bool> AddExperienceFromMessage(Character character, ulong channelId, string message);

        /// <summary>
        /// Gets the character's level.
        /// </summary>
        /// <param name="character">The character to calculate the level for.</param>
        /// <returns>An integer representing the character's level.</returns>
        int GetCharacterLevel(Character character);

        /// <summary>
        /// Shows important information when viewing a character related to the progression strategy.
        /// Example: How many more experience points the character needs before leveling up.
        /// </summary>
        /// <param name="character">The character being viewed.</param>
        /// <returns>Important information to show when viewing a character.</returns>
        Task<string> GetCharacterInfo(Character character);

        /// <summary>
        /// Shows important information when viewing a character's statistics related to the progression strategy.
        /// Example: How many attribute or skill points the character has remaining to spend.
        /// </summary>
        /// <param name="character">The character whose statistics are being viewed.</param>
        /// <returns>Important information related to the strategy to show when viewing a character's statistics.</returns>
        Task<string> GetCharacterStatisticsInfo(Character character);

        /// <summary>
        /// Sets the value of one of the character's statistics.
        /// </summary>
        /// <param name="character">The character to set the statistic value to.</param>
        /// <param name="statistic">The statistic to set the value to.</param>
        /// <param name="newValue">The optional value to bind with the statistic.</param>
        /// <exception cref="ProgressionException">
        /// Thrown when a character is not permitted to change the statistic to this value due to a lack of Experience, Level, etc...
        /// </exception>
        Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue = null);

        /// <summary>
        /// Sets whether a character's statistic is proficient or not
        /// </summary>
        /// <exception cref="ProgressionException">
        /// Thrown when a character is not permitted to set this statistic as proficient due to a lack of Experience, Level, etc...
        /// </exception>
        Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient);

        /// <summary>
        /// Resets a character to respec their statistics.
        /// </summary>
        Task<bool> ResetCharacter(Character character);
    }
}
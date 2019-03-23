using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Progression
{
    public interface IProgressionStrategy
    {
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
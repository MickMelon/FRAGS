using Frags.Core.Characters;

namespace Frags.Core.Statistics
{
    public interface IRollStrategy
    {
        /// <summary>
        /// Rolls a character's specified attribute
        /// and determines if they pass or fail the check.
        /// </summary>
        /// <returns>
        /// Returns a positive or negative integer representing 
        /// how good or bad a roll turned out.
        /// </returns>
        int? RollStatistic(Statistic stat, Character character);
    }
}
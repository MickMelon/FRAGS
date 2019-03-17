using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Rolling
{
    public interface IRollStrategy
    {
        /// <summary>
        /// Rolls a character's specified statistic
        /// and determines if they pass or fail the check.
        /// </summary>
        /// <returns>
        /// Returns a positive or negative number representing 
        /// how good or bad a roll turned out.
        /// </returns>
        double RollStatistic(Statistic stat, Character character);

        /// <summary>
        /// Rolls a character's specified statistic
        /// and creates a message to be sent to the view.
        /// </summary>
        /// <returns>
        /// Returns a string representing how good or bad a roll turned out.
        /// The returned string will be sent to the View.
        /// </returns>
        string GetRollMessage(Statistic stat, Character character);
    }
}
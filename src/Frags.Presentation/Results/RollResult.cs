using Frags.Core.Common;
using System.Text;

namespace Frags.Presentation.Results
{
    /// <summary>
    /// Represents a result type for rolls.
    /// </summary>
    public class RollResult : BaseResult
    {
        /// <summary>
        /// Initializes a new <see cref="RollResult" /> class.
        /// </summary>
        public RollResult(string message, bool success = true, object viewModel = null) 
            : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Returns a <see cref="RollResult" />.
        /// </summary>
        /// <param name="result">The result of the roll.</param>
        public static RollResult Roll(string result) =>
            new RollResult(result);

        /// <summary>
        /// Returns a <see cref="RollResult" />.
        /// </summary>
        public static RollResult RollFailed() =>
            new RollResult(Messages.ROLL_FAILED, false);

        /// <summary>
        /// Returns a <see cref="RollResult" />.
        /// </summary>
        /// <param name="char1">The first character's name.</param>
        /// <param name="char2">The second character's name.</param>
        /// <param name="roll1">The first character's roll.</param>
        /// <param name="roll2">The second character's roll.</param>
        public static RollResult RollAgainst(
            string character1, string character2, double roll1, double roll2)
        {
            if (roll1 > roll2)
                return new RollResult(
                    $"{character1} rolled {roll1} beating {character2}'s {roll2}!");

            return new RollResult(
                $"{character1} rolled {roll1} but failed to beat {character2}'s {roll2}");
        }            
    }
}
namespace Frags.Core.Controllers.Results
{
    /// <summary>
    /// Represents a result type for rolls.
    /// </summary>
    public class RollResult : BaseResult
    {
        /// <summary>
        /// Initializes a new <see cref="RollResult" /> class.
        /// </summary>
        public RollResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Returns a <see cref="RollResult" />.
        /// </summary>
        /// <param name="name">The character name.</param>
        /// <param name="stat">The rolled statistic.</param>
        /// <param name="roll">What the character rolled.</param>
        public static RollResult Roll(string name, string stat, int roll) =>
            new RollResult($"{name} rolled a {roll} in {stat}.");

        /// <summary>
        /// Returns a <see cref="RollResult" />.
        /// </summary>
        /// <param name="char1">The first character's name.</param>
        /// <param name="char2">The second character's name.</param>
        /// <param name="roll1">The first character's roll.</param>
        /// <param name="roll2">The second character's roll.</param>
        public static RollResult RollAgainst(string char1, string char2, int roll1, int roll2)
        {
            if (roll1 > roll2)
                return new RollResult($"{char1} rolled {roll1} beating {char2}'s {roll2}!");

            return new RollResult($"{char1} rolled {roll1} but failed to beat {char2}'s {roll2}");
        }            
    }
}
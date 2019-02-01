namespace Frags.Core.Controllers.Results
{
    public class RollResult : BaseResult
    {
        public RollResult(string message, bool success = true) : base(message, success)
        {
        }

        public static RollResult Roll(string name, string stat, int roll) =>
            new RollResult($"{name} rolled a {roll} in {stat}.");

        public static RollResult RollAgainst(string char1, string char2, int roll1, int roll2)
        {
            if (roll1 > roll2)
                return new RollResult($"{char1} rolled {roll1} beating {char2}'s {roll2}!");

            return new RollResult($"{char1} rolled {roll1} but failed to beat {char2}'s {roll2}");
        }
            
    }
}
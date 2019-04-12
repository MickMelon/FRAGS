using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Rolling
{
    public class MockRollStrategy : IRollStrategy
    {
        public string GetRollMessage(Statistic stat, Character character, bool useEffects = false)
        {
            return RollStatistic(stat, character).ToString();
        }

        /// </inheritdoc>
        public double? RollStatistic(Statistic stat, Character character, bool useEffects = false)
        {
            return character.GetStatistic(stat)?.Value;
        }
    }
}
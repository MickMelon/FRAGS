using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Rolling
{
    public class MockRollStrategy : IRollStrategy
    {
        /// </inheritdoc>
        public double? RollStatistic(Statistic stat, Character character)
        {
            return character.GetStatistic(stat).Value;
        }
    }
}
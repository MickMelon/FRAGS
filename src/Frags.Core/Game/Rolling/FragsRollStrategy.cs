using System;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Rolling
{
    public class FragsRollStrategy : IRollStrategy
    {
        /// </inheritdoc>
        public double? RollStatistic(Statistic stat, Character character)
        {
            int rng = GameRandom.Between(1, 100);
            double maxSuccessRoll;
            var statValue = character.Statistics[stat].Value;
            if (statValue <= 0) return -125;

            if (stat is Statistics.Attribute)
            {
                maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(statValue) - 7);
            }
            else
            {
                maxSuccessRoll = Math.Round(10 * Math.Sqrt(statValue) - 0.225 * statValue - 1);
            }

            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            
            return Math.Round(resultPercent * 100.0, 1);
        }
    }
}
using System;
using System.Linq;
using System.Text;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.Game.Rolling
{
    public class FragsRollStrategy : IRollStrategy
    {
        private static readonly double HIGHEST_MAX_SUCCESS_ROLL = 95;

        #region Messages
        private static readonly string ROLL_SUCCESS_CRIT = "**C R I T I C A L {0} SUCCESS!!!**";
        private static readonly string ROLL_SUCCESS_5 = "__***AMAZING {0} SUCCESS!***__";
        private static readonly string ROLL_SUCCESS_4 = "__GREAT {0} SUCCESS__";
        private static readonly string ROLL_SUCCESS_3 = "*Very good {0} success*";
        private static readonly string ROLL_SUCCESS_2 = "*Good {0} success*";
        private static readonly string ROLL_SUCCESS_1 = "*Above average {0} success*";
        private static readonly string ROLL_SUCCESS_0 = "__***CLOSE CALL! {0} success***__";

        private static readonly string ROLL_FAILURE_CRIT = "**Critical {0} failure!!!**";
        private static readonly string ROLL_FAILURE_5 = "__***GRUESOME {0} FAILURE!***___";
        private static readonly string ROLL_FAILURE_4 = "__TERRIBLE {0} FAILURE__";
        private static readonly string ROLL_FAILURE_3 = "*Pretty bad {0} failure*";
        private static readonly string ROLL_FAILURE_2 = "*Bad {0} failure*";
        private static readonly string ROLL_FAILURE_1 = "*Above average {0} failure*";
        private static readonly string ROLL_FAILURE_0 = "__***Heartbreaking {0} failure***__";
        
        private static readonly string ROLL_RESULT_SUCCESS = " for {0}: did **{1}%** better than needed!";
        private static readonly string ROLL_RESULT_FAILURE = " for {0}: did **{1}%** worse than needed!";

        private static readonly string USE_EFFECTS_EMOJI = "\uD83D\uDCAA";
        private static readonly string USE_EFFECTS_MESSAGE = $"{USE_EFFECTS_EMOJI} **USING EFFECTS!** {USE_EFFECTS_EMOJI}";
        #endregion

        /// </inheritdoc>
        public double? RollStatistic(Statistic stat, Character character, bool useEffects = false)
        {
            StatisticValue sv = character?.GetStatistic(stat, useEffects);
            if (character == null || sv == null) return null;

            int rng = GameRandom.Between(1, 100);
            double maxSuccessRoll;
            
            int statNum = sv.Value;
            if (statNum <= 0) return -999;

            if (stat is Attribute)
            {
                maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(statNum) - 7);
            }
            else
            {
                maxSuccessRoll = Math.Round(10 * Math.Sqrt(statNum) - 0.225 * statNum - 1);
            }

            Attribute luckStat = character.Statistics.Select(x => x.Statistic).OfType<Attribute>().FirstOrDefault(x => x.Aliases.ContainsIgnoreCase("luck"));
            double luckInfluence = 1.0;
            int luckDifference = 0;

            if (luckStat != null)
            {
                // each point of LCK above/below 5 prolly shoulda been +/-1% flat chance max
                // Do that, but ALSO make each point in luck a +/-0.1% chance for  mega crit

                int luckValue = character.GetStatistic(luckStat, useEffects).Value;
                luckDifference = luckValue - 5;

                luckInfluence += luckDifference * 0.01;
                maxSuccessRoll *= luckInfluence;
            }

            maxSuccessRoll = Math.Min(maxSuccessRoll, HIGHEST_MAX_SUCCESS_ROLL);

            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;

            int critRng = GameRandom.Between(1, 1000);
            // Success
            if (resultPercent >= 0)
            {
                double maxCritSuccessRoll = 1 + (1 * luckDifference);
                
                // Minimum 1, maximum 10
                maxCritSuccessRoll = Math.Max(1, Math.Min(maxCritSuccessRoll, 10));

                if (critRng <= maxCritSuccessRoll)
                    return 999;
            }
            // Failure
            else
            {
                double maxCritFailRoll = 6 - (1 * luckDifference);
                maxCritFailRoll = Math.Min(maxCritFailRoll, 10);
                
                if (critRng <= maxCritFailRoll)
                    return -999;
            }
            
            return Math.Round(resultPercent * 100.0, 1);
        }

        public string GetRollMessage(Statistic stat, Character character, bool useEffects = false)
        {
            var result = new StringBuilder();

            if (useEffects)
                result.Append(USE_EFFECTS_MESSAGE);

            var percent = RollStatistic(stat, character, useEffects);
            if (percent == null) return null;

            if (percent >= 0)
            {
                if (percent == 999)
                    result.Append(string.Format(ROLL_SUCCESS_CRIT, stat.Name.ToUpper()));
                else if (percent >= 125)
                    result.Append(string.Format(ROLL_SUCCESS_5, stat.Name.ToUpper()));
                else if (percent >= 80)
                    result.Append(string.Format(ROLL_SUCCESS_4, stat.Name.ToUpper()));
                else if (percent >= 50)
                    result.Append(string.Format(ROLL_SUCCESS_3, stat.Name));
                else if (percent >= 25)
                    result.Append(string.Format(ROLL_SUCCESS_2, stat.Name));
                else if (percent >= 10)
                    result.Append(string.Format(ROLL_SUCCESS_1, stat.Name));
                else
                    result.Append(string.Format(ROLL_SUCCESS_0, stat.Name));

                result.Append(string.Format(ROLL_RESULT_SUCCESS, character.Name, percent));
            }
            else
            {
                if (percent == 999)
                    result.Append(string.Format(ROLL_FAILURE_CRIT, stat.Name.ToUpper()));
                else if (percent <= -125)
                    result.Append(string.Format(ROLL_FAILURE_5, stat.Name.ToUpper()));
                else if (percent <= -80)
                    result.Append(string.Format(ROLL_FAILURE_4, stat.Name.ToUpper()));
                else if (percent <= -50)
                    result.Append(string.Format(ROLL_FAILURE_3, stat.Name));
                else if (percent <= -25)
                    result.Append(string.Format(ROLL_FAILURE_2, stat.Name));
                else if (percent <= -10)
                    result.Append(string.Format(ROLL_FAILURE_1, stat.Name));
                else
                    result.Append(string.Format(ROLL_FAILURE_0, stat.Name));

                result.Append(string.Format(ROLL_RESULT_FAILURE, character.Name, percent * -1));
            }

            return result.ToString();
        }
    }
}
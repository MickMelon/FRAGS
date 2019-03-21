using System;
using System.Text;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.Game.Rolling
{
    public class FragsRollStrategy : IRollStrategy
    {
        #region Messages
        private static readonly string ROLL_SUCCESS_5 = "**CRITICAL {0} SUCCESS!!!**";
        private static readonly string ROLL_SUCCESS_4 = "__GREAT {0} SUCCESS__";
        private static readonly string ROLL_SUCCESS_3 = "*Very good {0} success*";
        private static readonly string ROLL_SUCCESS_2 = "*Good {0} success*";
        private static readonly string ROLL_SUCCESS_1 = "*Above average {0} success*";
        private static readonly string ROLL_SUCCESS_0 = "__***CLOSE CALL! {0} success***__";
        
        private static readonly string ROLL_FAILURE_5 = "**CRITICAL {0} FAILURE!!!**";
        private static readonly string ROLL_FAILURE_4 = "__TERRIBLE {0} FAILURE__";
        private static readonly string ROLL_FAILURE_3 = "*Pretty bad {0} failure*";
        private static readonly string ROLL_FAILURE_2 = "*Bad {0} failure*";
        private static readonly string ROLL_FAILURE_1 = "*Above average {0} failure*";
        private static readonly string ROLL_FAILURE_0 = "__***Heartbreaking {0} failure***__";
        
        private static readonly string ROLL_RESULT_SUCCESS = " for {0}: did **{1}%** better than needed!";
        private static readonly string ROLL_RESULT_FAILURE = " for {0}: did **{1}%** worse than needed!";
        #endregion

        /// </inheritdoc>
        public double? RollStatistic(Statistic stat, Character character)
        {
            if (character == null || stat == null) return null;

            int rng = GameRandom.Between(1, 100);
            double maxSuccessRoll;

            int? statValue = character.GetStatistic(stat)?.Value;
            if (statValue == null) return null;
            if (statValue <= 0) return -125;

            if (stat is Attribute)
            {
                maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(statValue.Value) - 7);
            }
            else
            {
                maxSuccessRoll = Math.Round(10 * Math.Sqrt(statValue.Value) - 0.225 * statValue.Value - 1);
            }

            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            
            return Math.Round(resultPercent * 100.0, 1);
        }

        public string GetRollMessage(Statistic stat, Character character)
        {
            var result = new StringBuilder();
            var percent = RollStatistic(stat, character);
            if (percent == null) return null;

            if (percent >= 0)
            {
                if (percent >= 95)
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
                if (percent <= -125)
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
using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The statistic-value model.
    /// </summary>
    public class StatisticValue
    {
        public int Id { get; set; }

        /// <summary>
        /// The value associated with the statistic.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Determines whether to apply an (optional) bonus
        /// when increasing the value or using this Statistic
        /// </summary>
        public bool IsProficient { get; set; }

        /// <summary>
        /// Represents a bonus to add to a roll.
        /// </summary>
        public double Proficiency { get; set; }

        public override string ToString()
        {
            return Value + "," + IsProficient + "," + Proficiency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticValue" /> class.
        /// </summary>
        /// <param name="value">The value of the associated statistic.</param>
        /// <param name="isProficient">Whether or not to mark this statistic as proficient.</param>
        /// <param name="proficiency">The bonus that is applied if this statistic is marked proficient.</param>
        public StatisticValue(int value, bool isProficient = false, double proficiency = 0)
        {
            Value = value;
            IsProficient = isProficient;
            Proficiency = proficiency;
        }

        private StatisticValue() { }
    }
}
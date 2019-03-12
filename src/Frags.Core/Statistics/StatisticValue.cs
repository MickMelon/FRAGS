using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The statistic-value model.
    /// </summary>
    public struct StatisticValue
    {
        /// <summary>
        /// The value associated with the statistic.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Determines whether to apply an (optional) bonus
        /// when increasing the value or using this Statistic
        /// </summary>
        public bool IsProficient { get; set; }

        /// <summary>
        /// Represents a bonus to add to a roll.
        /// </summary>
        public double Proficiency { get; set; }
    }
}
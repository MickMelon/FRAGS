using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The statistic-value model.
    /// </summary>
    public class StatisticValue
    {
        /// <summary>
        /// The statistic associated with the value.
        /// </summary>
        public Statistic Statistic { get; set; }

        /// <summary>
        /// The value associated with the statistic.
        /// </summary>
        public int Value { get; set; }
    }
}
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Database.Statistics
{
    public class StatisticMapping
    {
        public string Id { get; set; }

        public Statistic Statistic { get; set; }
        public StatisticValue StatisticValue { get; set; }
    }
}
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Database.Statistics
{
    public class StatisticMapping
    {
        private StatisticMapping() {}

        public StatisticMapping(Statistic statistic, StatisticValue value)
        {
            Statistic = statistic;
            StatisticValue = value;
        }

        public string Id { get; set; }

        public Statistic Statistic { get; set; }
        public StatisticValue StatisticValue { get; set; }
    }
}
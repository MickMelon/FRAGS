using Frags.Core.Statistics;

namespace Frags.Database.Statistics
{
    public class StatisticMapping
    {
        private StatisticMapping() {}

        public StatisticMapping(StatisticDto statistic, StatisticValue value)
        {
            Statistic = statistic;
            StatisticValue = value;
        }

        public int Id { get; set; }

        public StatisticDto Statistic { get; set; }
        public StatisticValue StatisticValue { get; set; }
    }
}
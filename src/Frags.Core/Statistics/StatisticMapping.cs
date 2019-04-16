namespace Frags.Core.Statistics
{
    public class StatisticMapping
    {
        private StatisticMapping() {}

        public StatisticMapping(Statistic statistic, StatisticValue value)
        {
            Statistic = statistic;
            StatisticValue = value;
        }

        public int Id { get; set; }

        public Statistic Statistic { get; set; }
        public StatisticValue StatisticValue { get; set; }
    }
}
using Frags.Core.Characters;
using Frags.Core.Effects;

namespace Frags.Core.Statistics
{
    public class StatisticMapping
    {
        public StatisticMapping() { }

        public StatisticMapping(Statistic statistic, StatisticValue value)
        {
            Statistic = statistic;
            StatisticValue = value;
        }

        public int Id { get; set; }

        public Statistic Statistic { get; set; }
        public StatisticValue StatisticValue { get; set; }

        public int? CharacterId { get; set; }
        public Character Character { get; set; }

        public int? EffectId { get; set; }
        public Effect Effect { get; set; }
    }
}
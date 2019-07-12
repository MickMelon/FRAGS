using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect
    {
        protected Effect() { }

        public Effect(ulong ownerId, string name)
        {
            StatisticEffects = new Dictionary<Statistic, StatisticValue>();
            Name = name;
            OwnerUserIdentifier = ownerId;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        public virtual Dictionary<Statistic, StatisticValue> StatisticEffects { get; set; }
    }
}

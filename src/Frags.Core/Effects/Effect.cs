using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect
    {
        protected Effect() { }

        public Effect(User owner, string name)
        {
            StatisticEffects = new Dictionary<Statistic, StatisticValue>();
            Name = name;
            Owner = owner;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public User Owner { get; set; }

        public virtual Dictionary<Statistic, StatisticValue> StatisticEffects { get; set; }
    }
}

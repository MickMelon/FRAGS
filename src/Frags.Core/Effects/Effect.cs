using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect
    {
        protected Effect() { }

        public Effect(string name, ulong ownerId)
        {
            StatisticEffects = new List<StatisticMapping>();
            Name = name;
            OwnerUserIdentifier = ownerId;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        public virtual IList<StatisticMapping> StatisticEffects { get; set; }
    }
}

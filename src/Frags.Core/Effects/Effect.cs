using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        public virtual IList<StatisticMapping> StatisticEffects { get; set; }

        public virtual IList<EffectMapping> EffectMappings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Statistics.Effects
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

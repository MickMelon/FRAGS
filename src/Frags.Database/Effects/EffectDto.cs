using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Statistics;

namespace Frags.Database.Effects
{
    public class EffectDto
    {
        public EffectDto ()
        {
            StatisticEffects = new List<StatisticMapping>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        public virtual IList<StatisticMapping> StatisticEffects { get; set; }

        public virtual IList<EffectMapping> EffectMappings { get; set; }
    }
}

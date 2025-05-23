using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect
    {
        protected Effect() { }

        public Effect(ulong ownerId, string name)
        {
            StatisticEffects = new List<StatisticMapping>();
            Name = name;
            OwnerUserIdentifier = ownerId;
            Characters = new List<Character>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        public virtual IList<StatisticMapping> StatisticEffects { get; set; }
        public List<Character> Characters { get; set; }
    }
}

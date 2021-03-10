using System;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;

namespace Frags.Core.Effects
{
    public class Effect : IStatsheetContainer
    {
        protected Effect() { }

        public Effect(User owner, string name, Campaign campaign)
        {
            Statistics = new Dictionary<Statistic, StatisticValue>();
            Name = name;
            Owner = owner;
            Campaign = campaign;
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public User Owner { get; set; }

        public Campaign Campaign { get; set; }

        /// <summary>
        /// The effect on a Statsheet after it is applied.
        /// StatisticValues will be added on top of existing values.
        /// </summary>
        public Dictionary<Statistic, StatisticValue> Statistics { get; set; }
    }
}

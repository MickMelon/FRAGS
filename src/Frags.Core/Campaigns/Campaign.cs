using System.Collections.Generic;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.Campaigns
{
    public class Campaign
    {
        public ulong OwnerUserIdentifier { get; set; }
        public ICollection<ulong> ModeratorUserIdentifiers { get; set; }

        public ICollection<ulong> Channels { get; set; }

        public ICollection<Character> Characters { get; set; }
        public ICollection<Effect> Effects { get; set; }
        public ICollection<Statistic> Statistics { get; set; }

        public RollOptions RollOptions { get; set; }
        public StatisticOptions StatisticOptions { get; set; }
    }
}
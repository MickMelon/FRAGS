using System.Collections.Generic;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.Campaigns
{
    public class Campaign
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ulong OwnerUserIdentifier { get; set; }
        public ICollection<ulong> ModeratorUserIdentifiers { get; set; }

        public ICollection<ulong> Channels { get; set; }

        public ICollection<Character> Characters { get; set; }
        public ICollection<Effect> Effects { get; set; }
        public ICollection<Statistic> Statistics { get; set; }

        public RollOptions RollOptions { get; set; }
        public StatisticOptions StatisticOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Campaign" /> class.
        /// </summary>
        /// <param name="ownerIdentifier">The unique identifier of user that owns the campaign.</param>
        /// <param name="name">The campaigns's name.</param>
        public Campaign(ulong ownerIdentifier, string name)
        {
            OwnerUserIdentifier = ownerIdentifier;
            Name = name;

            ModeratorUserIdentifiers = new List<ulong>();
            Channels = new List<ulong>();
            Characters = new List<Character>();
            Statistics = new List<Statistic>();
            Effects = new List<Effect>();
        }
    }
}
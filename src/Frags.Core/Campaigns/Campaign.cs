using System.Collections.Generic;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.Campaigns
{
    public class Campaign
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public User Owner { get; set; }
        public List<Moderator> ModeratedCampaigns { get; set; }

        public List<Channel> Channels { get; set; }

        public List<Character> Characters { get; set; }
        public List<Effect> Effects { get; set; }
        public List<Statistic> Statistics { get; set; }

        public RollOptions RollOptions { get; set; }
        public StatisticOptions StatisticOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Campaign" /> class.
        /// </summary>
        /// <param name="owner">The unique identifier of user that owns the campaign.</param>
        /// <param name="name">The campaigns's name.</param>
        public Campaign(User owner, string name, Channel firstChannel)
        {
            Owner = owner;
            Name = name;

            ModeratedCampaigns = new List<Moderator>();
            Channels = new List<Channel> { firstChannel };
            Characters = new List<Character>();
            Statistics = new List<Statistic>();
            Effects = new List<Effect>();
        }

        protected Campaign() { }
    }
}
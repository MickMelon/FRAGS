using System.Collections.Generic;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Frags.Database.Statistics;

namespace Frags.Database.Campaigns
{
    public class CampaignDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ulong OwnerUserIdentifier { get; set; }

        // Many to Many
        public ICollection<Moderator> Moderators { get; set; }

        // Many to one
        public ICollection<ChannelDto> Channels { get; set; }

        // Many to one
        public ICollection<CharacterDto> Characters { get; set; }

        // Many to one
        public ICollection<EffectDto> Effects { get; set; }

        // Many to one
        public ICollection<Statistic> Statistics { get; set; }

        // One to one
        public RollOptionsDto RollOptions { get; set; }

        // One to one
        public StatisticOptionsDto StatisticOptions { get; set; }
    }
}
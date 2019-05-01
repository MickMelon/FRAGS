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

        public ulong OwnerUserIdentifier { get; set; }
        public ICollection<Moderator> Moderators { get; set; }

        public ICollection<ChannelDto> Channels { get; set; }

        public ICollection<CharacterDto> Characters { get; set; }
        public ICollection<EffectDto> Effects { get; set; }
        public ICollection<Statistic> Statistics { get; set; }

        public RollOptionsDto RollOptions { get; set; }
        public StatisticOptionsDto StatisticOptions { get; set; }
    }
}
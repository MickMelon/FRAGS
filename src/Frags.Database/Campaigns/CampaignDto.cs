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

        public UserDto Owner { get; set; }

        // Many to many
        public List<ModeratorDto> ModeratedCampaigns { get; set; }

        // Many to one
        public List<ChannelDto> Channels { get; set; }

        // Many to one
        public List<CharacterDto> Characters { get; set; }

        // Many to one
        public List<EffectDto> Effects { get; set; }

        // Many to one
        public List<StatisticDto> Statistics { get; set; }

        // One to one
        public RollOptionsDto RollOptions { get; set; }

        // One to one
        public StatisticOptionsDto StatisticOptions { get; set; }

        public CampaignDto()
        {
            ModeratedCampaigns = new List<ModeratorDto>();
            Channels = new List<ChannelDto>();
            Characters = new List<CharacterDto>();
            Effects = new List<EffectDto>();
            Statistics = new List<StatisticDto>();
        }
    }
}
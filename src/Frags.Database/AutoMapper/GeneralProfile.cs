using System.Linq;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Frags.Database.Statistics;

namespace Frags.Database.AutoMapper
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            // Can't go from a collection of POCO Users to Moderator many-to-many DTOs...
            CreateMap<Campaign, CampaignDto>();

            // ...but we can separate the UserDTO's from the Moderator object
            CreateMap<CampaignDto, Campaign>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Character, CharacterDto>()
               .ForMember(x => x.Statistics, opt => opt.Ignore());
            CreateMap<CharacterDto, Character>()
               .ForMember(poco => poco.Attributes, opt => opt.Ignore())
               .ForMember(poco => poco.Skills, opt => opt.Ignore())
               .ForMember(poco => poco.Effects, opt => opt.Ignore())
               .ForMember(x => x.Statistics, opt => opt.Ignore());

            CreateMap<Effect, EffectDto>();
            CreateMap<EffectDto, Effect>();

            CreateMap<Statistic, StatisticDto>()
               .Include<Attribute, AttributeDto>()
               .Include<Skill, SkillDto>();

            CreateMap<StatisticDto, Statistic>()
               .Include<AttributeDto, Attribute>()
               .Include<SkillDto, Skill>();

            CreateMap<Skill, SkillDto>();
            CreateMap<SkillDto, Skill>();

            CreateMap<Attribute, AttributeDto>();
            CreateMap<AttributeDto, Attribute>();

            CreateMap<StatisticOptions, StatisticOptionsDto>();
            CreateMap<StatisticOptionsDto, StatisticOptions>();
               
            CreateMap<RollOptions, RollOptionsDto>();
            CreateMap<RollOptionsDto, RollOptions>();

            CreateMap<Channel, ChannelDto>();
            CreateMap<ChannelDto, Channel>();
        }
    }
}
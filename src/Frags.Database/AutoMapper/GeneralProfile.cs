using System.Collections.Generic;
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
            CreateMap<Campaign, CampaignDto>();
            CreateMap<CampaignDto, Campaign>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<KeyValuePair<Statistic, StatisticValue>, StatisticMapping>()
               .ConvertUsing<StatisticKeyValueConverter>();

            CreateMap<StatisticMapping, KeyValuePair<Statistic, StatisticValue>>()
               .ConvertUsing<StatisticMappingConverter>();

            CreateMap<Character, CharacterDto>()
               .ForMember(x => x.EffectMappings, opt => opt.MapFrom<EffectMappingResolver>());

            CreateMap<CharacterDto, Character>()
               .ForMember(poco => poco.Attributes, opt => opt.Ignore())
               .ForMember(poco => poco.Skills, opt => opt.Ignore())
               .ForMember(poco => poco.Effects, opt => opt.MapFrom<EffectPocoResolver>());

            CreateMap<Effect, EffectDto>()
               .ForMember(dto => dto.EffectMappings, opt => opt.Ignore());
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
using System.Collections.Generic;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Database.Characters;
using Frags.Database.Effects;

namespace Frags.Database.AutoMapper
{
    public class EffectMappingResolver : IValueResolver<Character, CharacterDto, IList<EffectMapping>>
    {
        public IList<EffectMapping> Resolve(Character source, CharacterDto destination, IList<EffectMapping> destMember, ResolutionContext context)
        {
            IList<EffectMapping> list = new List<EffectMapping>();

            foreach (var effect in source.Effects)
            {
                EffectDto effectDto = context.Mapper.Map<EffectDto>(effect);

                list.Add(new EffectMapping
                {
                    Character = destination,
                    //CharacterId = destination.Id,
                    Effect = effectDto,
                    //EffectId = effectDto.Id
                });
            }

            return list;
        }
    }

    public class EffectPocoResolver : IValueResolver<CharacterDto, Character, IList<Effect>>
    {
        public IList<Effect> Resolve(CharacterDto source, Character destination, IList<Effect> destMember, ResolutionContext context)
        {
            IList<Effect> list = new List<Effect>();
            if (source.EffectMappings == null) return null;

            foreach (var effectMap in source.EffectMappings)
                list.Add(context.Mapper.Map<Effect>(effectMap.Effect));

            return list;
        }
    }
}
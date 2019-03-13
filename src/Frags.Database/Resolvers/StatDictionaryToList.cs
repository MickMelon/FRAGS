using System.Collections.Generic;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Frags.Database.Statistics;

namespace Frags.Database.Resolvers
{
    public class StatDictionaryToList : IValueResolver<Character, CharacterDto, IList<StatisticMapping>>
    {
        public IList<StatisticMapping> Resolve(Character source, CharacterDto destination, IList<StatisticMapping> destMember, ResolutionContext context)
        {
            var list = new List<StatisticMapping>();
            foreach (var stat in source.Statistics)
            {
                list.Add(new StatisticMapping
                {
                    Statistic = stat.Key,
                    StatisticValue = stat.Value
                });
            }
            return list;
        }
    }
}
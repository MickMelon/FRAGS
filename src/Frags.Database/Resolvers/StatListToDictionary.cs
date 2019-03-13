using System.Collections.Generic;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database.Characters;

namespace Frags.Database.Resolvers
{
    public class StatListToDictionary : IValueResolver<CharacterDto, Character, IDictionary<Statistic, StatisticValue>>
    {
        public IDictionary<Statistic, StatisticValue> Resolve(CharacterDto source, Character destination, IDictionary<Statistic, StatisticValue> destMember, ResolutionContext context)
        {
            var dict = new Dictionary<Statistic, StatisticValue>();

            foreach (var stat in source.StatisticMappings)
                dict.Add(stat.Statistic, stat.StatisticValue);

            return dict;
        }
    }
}
using System.Collections.Generic;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Frags.Database.Statistics;

namespace Frags.Database.AutoMapper
{
    public class StatisticKeyValueConverter : ITypeConverter<KeyValuePair<Statistic, StatisticValue>, StatisticMapping>
    {
        public StatisticMapping Convert(KeyValuePair<Statistic, StatisticValue> source, StatisticMapping destination, ResolutionContext context)
        {
            return new StatisticMapping(context.Mapper.Map<StatisticDto>(source.Key), source.Value);
        }
    }

    public class StatisticMappingConverter : ITypeConverter<StatisticMapping, KeyValuePair<Statistic, StatisticValue>>
    {
        public KeyValuePair<Statistic, StatisticValue> Convert(StatisticMapping source, KeyValuePair<Statistic, StatisticValue> destination, ResolutionContext context)
        {
            return new KeyValuePair<Statistic, StatisticValue>(context.Mapper.Map<Statistic>(source.Statistic), source.StatisticValue);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Database.Models;

namespace Frags.Database.DataAccess
{
    internal class DbHelper
    {
        internal static async Task<Dictionary<Statistic, StatisticValue>> GetStatisticDictionary(StatisticList statlist, IStatisticProvider statProvider)
        {
            var result = new Dictionary<Statistic, StatisticValue>();

            if (statlist == null || string.IsNullOrWhiteSpace(statlist.Data))
                return result;

            var deserialized = JsonSerializer.Deserialize<Dictionary<int,StatisticValue>>(statlist.Data);

            foreach (var statmap in deserialized)
            {
                Statistic stat = await statProvider.GetStatisticAsync(statmap.Key);
                if (stat == null) continue;

                result.Add(stat, statmap.Value);
            }

            return result;
        }

        internal static string SerializeStatisticList(Dictionary<Statistic, StatisticValue> dict)
        {
            if (dict == null || dict.Count <= 0)
                return string.Empty;

            var newDict = new Dictionary<int, StatisticValue>();
            foreach (var statmap in dict)
                newDict.Add(statmap.Key.Id, statmap.Value);

            return JsonSerializer.Serialize(newDict);
        }

        internal static async Task<IList<Effect>> GetEffectList(EffectList effectlist, IEffectProvider effectProvider)
        {
            IList<Effect> result = new List<Effect>();

            if (effectlist == null || string.IsNullOrWhiteSpace(effectlist.Data))
                return result;

            int[] ids = effectlist.Data.Split(',').Select(int.Parse).ToArray();

            foreach (int id in ids)
            {
                Effect effect = await effectProvider.GetEffectAsync(id);
                if (effect == null) continue;

                result.Add(effect);
            }

            return result;
        }

        internal static string SerializeEffectList(IList<Effect> effects)
        {
            return string.Join(",", effects.Select(x => x.Id));
        }
    }
}
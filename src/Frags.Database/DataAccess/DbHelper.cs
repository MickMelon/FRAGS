using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Frags.Core.DataAccess;
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
    }
}
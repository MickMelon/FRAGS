using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    internal class DbHelper
    {
        internal static async Task<Dictionary<Statistic, StatisticValue>> GetStatisticDictionary(StatisticList statlist, IStatisticProvider statProvider)
        {
            var result = new Dictionary<Statistic, StatisticValue>();

            if (statlist == null || string.IsNullOrWhiteSpace(statlist.Data))
                return result;

            var deserialized = JsonSerializer.Deserialize<Dictionary<int, StatisticValue>>(statlist.Data);

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

        internal static async Task EfUpdateOrCreateEffectList(Character character, RpgContext context)
        {
            EffectList effectList = await context.EffectLists.FirstOrDefaultAsync(x => x.CharacterId == character.Id);
            if (character.Effects != null)
            {
                string data = DbHelper.SerializeEffectList(character.Effects);

                if (effectList != null)
                {
                    effectList.Data = data;
                    context.Update(effectList);
                }
                else
                {
                    effectList = new EffectList(character);
                    effectList.Data = data;
                    context.Add(effectList);
                }
            }
        }

        internal static async Task EfUpdateOrCreateStatisticList(Character character, RpgContext context)
        {
            StatisticList statlist = await context.StatisticLists.FirstOrDefaultAsync(x => x.CharacterId == character.Id);
            EfUpdateOrCreateStatisticList(statlist, character, context);
        }

        internal static async Task EfUpdateOrCreateStatisticList(Effect effect, RpgContext context)
        {
            StatisticList statlist = await context.StatisticLists.FirstOrDefaultAsync(x => x.EffectId == effect.Id);
            EfUpdateOrCreateStatisticList(statlist, effect, context);
        }

        private static void EfUpdateOrCreateStatisticList(StatisticList statlist, IStatsheetContainer statContainer, RpgContext context)
        {
            if (statContainer.Statistics != null)
            {
                string data = DbHelper.SerializeStatisticList(statContainer.Statistics);

                if (statlist != null)
                {
                    statlist.Data = data;
                    context.Update(statlist);
                }
                else
                {
                    if (statContainer is Character ch)
                        statlist = new StatisticList(ch);
                    else if (statContainer is Effect ef)
                        statlist = new StatisticList(ef);
                    else
                        throw new NotImplementedException();

                    statlist.Data = data;
                    context.Add(statlist);
                }
            }
        }
    }
}
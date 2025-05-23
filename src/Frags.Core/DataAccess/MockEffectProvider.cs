using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.Effects;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    public class MockEffectProvider : IEffectProvider
    {
        private readonly List<Effect> _effects = new List<Effect>();

        private int id = 1;

        public Task AddEffectToCharacter(Effect effect, Character character)
        {
            character.Effects.Add(effect);
            return Task.CompletedTask;
        }

        public Task<Effect> CreateEffectAsync(ulong ownerId, string name)
        {
            var effect = new Effect(ownerId, name) { Id = id++ };
            _effects.Add(effect);
            return Task.FromResult(effect);
        }

        public Task DeleteEffectAsync(Effect effect)
        {
            _effects.Remove(effect);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Effect>> GetAllEffectsAsync()
        {
            return Task.FromResult<IEnumerable<Effect>>(_effects);
        }

        public Task<Effect> GetEffectAsync(string name)
        {
            return Task.FromResult(_effects.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name)));
        }

        public Task<IEnumerable<Effect>> GetUserEffectsAsync(ulong userId)
        {
            return Task.FromResult<IEnumerable<Effect>>(_effects.Where(x => x.OwnerUserIdentifier == userId).ToList());
        }

        public Task LoadStatistics(Effect effect)
        {
            return Task.CompletedTask;
        }

        public Task SetStatisticEffect(Effect effect, Statistic stat, StatisticValue statVal)
        {
            var dbEffect = _effects[_effects.FindIndex(x => x.Id.Equals(effect.Id))];
            var statMap = dbEffect.StatisticEffects.FirstOrDefault(x => x.Statistic.Equals(stat));
            if (statMap == null)
            {
                dbEffect.StatisticEffects.Add(new StatisticMapping()
                {
                    Effect = effect,
                    EffectId = effect.Id,
                    Statistic = stat,
                    StatisticValue = statVal
                });
            }
            else
            {
                statMap.StatisticValue = statVal;
            }
            return Task.CompletedTask;
        }

        public Task UpdateEffectAsync(Effect effect)
        {
            _effects[_effects.FindIndex(x => x.Id.Equals(effect.Id))] = effect;
            return Task.CompletedTask;
        }
    }
}
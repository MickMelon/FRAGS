using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common.Extensions;
using Frags.Core.Effects;

namespace Frags.Core.DataAccess
{
    public class MockEffectProvider : IEffectProvider
    {
        private readonly List<Effect> _effects = new List<Effect>();

        private int id = 1;

        public Task<Effect> CreateEffectAsync(string name)
        {
            var effect = new Effect { Id = (id++).ToString(), Name = name };
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

        public Task UpdateEffectAsync(Effect effect)
        {
            _effects[_effects.FindIndex(x => x.Id.Equals(effect.Id))] = effect;
            return Task.CompletedTask;
        }
    }
}
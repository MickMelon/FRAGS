using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfEffectProvider : IEffectProvider
    {
        private readonly RpgContext _context;

        public EfEffectProvider(RpgContext context)
        {
            _context = context;
        }

        public async Task<Effect> CreateEffectAsync(string name)
        {
            var effect = new Effect { Name = name };
            await _context.AddAsync(effect);
            await _context.SaveChangesAsync();
            return effect;
        }

        public async Task DeleteEffectAsync(Effect effect)
        {
            _context.Remove(effect);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Effect>> GetAllEffectsAsync()
        {
            return await _context.Effects.ToListAsync();
        }

        public async Task<Effect> GetEffectAsync(string name)
        {
            return await _context.Effects.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(name));
        }

        public async Task UpdateEffectAsync(Effect effect)
        {
            _context.Update(effect);
            await _context.SaveChangesAsync();
        }
    }
}
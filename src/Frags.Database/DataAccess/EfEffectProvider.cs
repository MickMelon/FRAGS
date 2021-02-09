using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfEffectProvider : IEffectProvider
    {
        private readonly RpgContext _context;

        private readonly IUserProvider _userProvider;

        public EfEffectProvider(RpgContext context, IUserProvider userProvider)
        {
            _context = context;

            _userProvider = userProvider;
        }

        public async Task<Effect> CreateEffectAsync(ulong ownerId, string name)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == ownerId);

            if (user == null)
                user = await _userProvider.CreateUserAsync(ownerId);

            Effect effect = new Effect(user, name);

            await _context.Effects.AddAsync(effect);
            await _context.SaveChangesAsync();
            
            return effect;
        }

        public async Task DeleteEffectAsync(Effect effect)
        {
            // If it doesn't exist in the DB, abort
            if (await _context.Effects.CountAsync(c => c.Id.Equals(effect.Id)) <= 0)
                return;

            var dto = await _context.Effects.FirstOrDefaultAsync(x => x.Id.Equals(effect.Id));
            _context.Remove(dto);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Effect>> GetAllEffectsAsync()
        {
            var effectList = await _context.Effects.ToListAsync();
            //foreach (var effect in effectList)
                //effect.StatisticEffects = ...

            return effectList;
        }

        public async Task<Effect> GetEffectAsync(string name)
        {
            return await _context.Effects
                .Where(x => x.Name.EqualsIgnoreCase(name))
                // .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                // .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Effect>> GetOwnedEffectsAsync(ulong userId)
        {
            return await _context.Effects
                .Where(x => x.Owner.UserIdentifier == userId)
                // .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                // .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .ToListAsync();
        }

        public async Task UpdateEffectAsync(Effect effect)
        {
            // If it doesn't exist in the DB, abort
            if (await _context.Effects.CountAsync(c => c.Id.Equals(effect.Id)) <= 0)
                return;

            _context.Update(effect);
            await _context.SaveChangesAsync();
        }
    }
}
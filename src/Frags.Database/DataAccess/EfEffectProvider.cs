using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Statistics;
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

        public async Task AddEffectToCharacter(Effect effect, Character character)
        {
            _context.ChangeTracker.Clear();
            await _context.EffectMappings.AddAsync(new EffectMapping
            {
                CharacterId = character.Id,
                EffectId = effect.Id
            });
            await _context.SaveChangesAsync();
        }

        public async Task<Effect> CreateEffectAsync(ulong ownerId, string name)
        {
            var effect = new Effect(ownerId, name);

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
            return await _context.Effects
                .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .ToListAsync();
        }

        public async Task<Effect> GetEffectAsync(string name)
        {
            return await _context.Effects
                .AsAsyncEnumerable()
                .Where(x => x.Name.EqualsIgnoreCase(name))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Effect>> GetUserEffectsAsync(ulong userId)
        {
            return await _context.Effects
                .Where(x => x.OwnerUserIdentifier == userId)
                .ToListAsync();
        }

        public async Task LoadStatistics(Effect effect)
        {
            await _context.Entry(effect).Collection(x => x.StatisticEffects).Query()
                .Include(x => x.Statistic)
                .Include(x => x.StatisticValue)
                .LoadAsync();
        }

        public async Task SetStatisticEffect(Effect effect, Statistic stat, StatisticValue statVal)
        {
            await LoadStatistics(effect);
            _context.ChangeTracker.Clear();
            var statMap = effect.StatisticEffects.FirstOrDefault(x => x.Statistic.Equals(stat));
            if (statMap == null)
            {
                statMap = new StatisticMapping()
                {
                    Effect = effect,
                    Statistic = stat,
                    StatisticValue = statVal
                };
            }
            else
            {
                statMap.StatisticValue = statVal;
            }

            // _context.Entry(statMap.Statistic).State = EntityState.Detached;
            _context.Update(statMap);
            await _context.SaveChangesAsync();
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
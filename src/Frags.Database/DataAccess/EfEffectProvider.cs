using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Database.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfEffectProvider : IEffectProvider
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        public EfEffectProvider(RpgContext context, IMapper mapper)
        {
            _context = context;
            
            _mapper = mapper;
        }

        public async Task<Effect> CreateEffectAsync(ulong ownerId, string name)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == ownerId);
            var userMapped = _mapper.Map<User>(userDto);

            var effect = new Effect(userMapped, name);
            var dto = _mapper.Map<EffectDto>(effect);

            await _context.Effects.AddAsync(dto);
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
            return _mapper.Map<List<Effect>>(await _context.Effects
                .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .ToListAsync());
        }

        public async Task<Effect> GetEffectAsync(string name)
        {
            return _mapper.Map<Effect>(await _context.Effects
                .Where(x => x.Name.EqualsIgnoreCase(name))
                .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<Effect>> GetOwnedEffectsAsync(ulong userId)
        {
            return _mapper.Map<List<Effect>>(await _context.Effects
                .Where(x => x.Owner.UserIdentifier == userId)
                .Include(x => x.StatisticEffects).ThenInclude(y => y.Statistic)
                .Include(x => x.StatisticEffects).ThenInclude(y => y.StatisticValue)
                .ToListAsync());
        }

        public async Task UpdateEffectAsync(Effect effect)
        {
            // If it doesn't exist in the DB, abort
            if (await _context.Effects.CountAsync(c => c.Id.Equals(effect.Id)) <= 0)
                return;

            var dto = await _context.Effects.FirstOrDefaultAsync(x => x.Id.Equals(effect.Id));
            _mapper.Map<Effect, EffectDto>(effect, dto);
            dto.EffectMappings = await _context.Set<EffectMapping>().Where(x => x.EffectId.Equals(effect.Id)).ToListAsync();
            _context.Update(dto);

            await _context.SaveChangesAsync();
        }
    }
}
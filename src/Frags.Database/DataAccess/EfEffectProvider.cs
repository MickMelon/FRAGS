using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfEffectProvider : IEffectProvider
    {
        private readonly RpgContext _context;

        private readonly IUserProvider _userProvider;
        private readonly IStatisticProvider _statProvider;

        public EfEffectProvider(RpgContext context, IUserProvider userProvider, IStatisticProvider statProvider)
        {
            _context = context;

            _userProvider = userProvider;
            _statProvider = statProvider;
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

        public async Task<Effect> GetEffectAsync(int id)
        {
            Effect effect = await _context.Effects.FirstOrDefaultAsync(x => x.Id == id);
            if (effect == null) return null;

            StatisticList statlist = await _context.StatisticLists.FirstOrDefaultAsync(x => x.EffectId == id);
            effect.StatisticEffects = await DbHelper.GetStatisticDictionary(statlist, _statProvider);

            return effect;
        }

        public async Task<Effect> GetEffectAsync(string name)
        {
            Effect effect = await _context.Effects.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(name));
            if (effect == null) return null;

            StatisticList statlist = await _context.StatisticLists.FirstOrDefaultAsync(x => x.EffectId == effect.Id);
            effect.StatisticEffects = await DbHelper.GetStatisticDictionary(statlist, _statProvider);

            return effect;
        }

        public async Task<IEnumerable<Effect>> GetOwnedEffectsAsync(ulong userId)
        {
            List<Effect> effects = await _context.Effects.Include(x => x.Owner).Where(x => x.Owner.UserIdentifier == userId).ToListAsync();
            if (effects == null || effects.Count <= 0) return effects;

            foreach (var effect in effects)
            {
                StatisticList statlist = await _context.StatisticLists.FirstOrDefaultAsync(x => x.EffectId == effect.Id);
                effect.StatisticEffects = await DbHelper.GetStatisticDictionary(statlist, _statProvider);    
            }

            return effects;
        }

        public async Task UpdateEffectAsync(Effect effect)
        {
            StatisticList statlist = await _context.StatisticLists.FirstOrDefaultAsync(x => x.EffectId == effect.Id);
            if (effect.StatisticEffects != null)
            {
                string data = DbHelper.SerializeStatisticList(effect.StatisticEffects);
                
                if (statlist != null)
                {
                    statlist.Data = data;
                    _context.Update(statlist);
                }
                else
                {
                    statlist = new StatisticList(effect);
                    statlist.Data = data;
                    _context.Add(statlist);
                }
            }

            _context.Update(effect);
            await _context.SaveChangesAsync();
        }
    }
}
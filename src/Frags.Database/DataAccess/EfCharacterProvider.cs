using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Core.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using Frags.Database.Models;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly RpgContext _context;

        private readonly IUserProvider _userProvider;
        private readonly IStatisticProvider _statProvider;
        private readonly IEffectProvider _effectProvider;

        public EfCharacterProvider(RpgContext context, IUserProvider userProvider, IStatisticProvider statProvider, IEffectProvider effectProvider)
        {
            _context = context;
            _userProvider = userProvider;
            _statProvider = statProvider;
            _effectProvider = effectProvider;
        }

        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == discordId);

            if (user == null)
            {
                await _userProvider.CreateUserAsync(discordId);
                user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == discordId);
            }

            Character character = new Character(user, name);
            await _context.AddAsync(character);

            user.ActiveCharacter = character;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            User user = await _context.Users.Include(x => x.ActiveCharacter).FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);

            if (user?.ActiveCharacter != null)
            {
                await LoadRelatedCharacterData(user.ActiveCharacter);
                return user.ActiveCharacter;
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            User user = await _context.Users.Include(x => x.Characters).FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);
            if (user == null) return null;

            List<Character> characters = user.Characters;

            characters.ForEach(async character => await LoadRelatedCharacterData(character));

            return characters;
        }

        private async Task LoadRelatedCharacterData(Character character)
        {
            await _context.Entry(character).Reference(x => x.User).LoadAsync();
            await _context.Entry(character).Reference(x => x.Campaign).LoadAsync();

            StatisticList statlist = await _context.StatisticLists.FirstOrDefaultAsync(x => x.CharacterId == character.Id);
            character.Statistics = await DbHelper.GetStatisticDictionary(statlist, _statProvider);

            EffectList effectList = await _context.EffectLists.FirstOrDefaultAsync(x => x.CharacterId == character.Id);
            character.Effects = await DbHelper.GetEffectList(effectList, _effectProvider);
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            await DbHelper.EfUpdateOrCreateStatisticList(character, _context);
            await DbHelper.EfUpdateOrCreateEffectList(character, _context);

            _context.Update(character);
            await _context.SaveChangesAsync();
        }
    }
}
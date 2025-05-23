using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly RpgContext _context;

        public EfCharacterProvider(RpgContext context)
        {
            _context = context;
        }

        private async Task<Character> CreateCharacterAsync(Character character)
        {
            // Check the database for a character with the same ID as the new one
            // If one exists, don't add it
            if (await _context.Characters.CountAsync(x => x.Id.Equals(character.Id)) > 0)
                return null;

            await _context.AddAsync(character);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

            if (user == null)
            {
                await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = character });
            }
            else if (character.Active)
            {
                user.ActiveCharacter = character;
            }

            await _context.SaveChangesAsync();
            return character;
        }

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(ulong userIdentifier, string name) =>
            await CreateCharacterAsync(new Character(userIdentifier, name));

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(int id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "") =>
            await CreateCharacterAsync(new Character(id, userIdentifier, active, name, description, story));

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            var dto = await _context.Users.Where(c => c.UserIdentifier == userIdentifier)
                // .Include(x => x.ActiveCharacter).ThenInclude(x => x.Statistics).ThenInclude(x => x.Statistic)
                // .Include(x => x.ActiveCharacter).ThenInclude(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                // .Include(x => x.ActiveCharacter).ThenInclude(x => x.Effects).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                // .Include(x => x.ActiveCharacter).ThenInclude(x => x.Effects).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .Select(usr => usr.ActiveCharacter)
                .FirstOrDefaultAsync();

            if (dto == null) return null;
            dto.Active = true;
            return dto;
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            var characters = await _context.Characters.Where(c => c.UserIdentifier == userIdentifier)
                // .Include(x => x.Statistics).ThenInclude(x => x.Statistic)
                // .Include(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                // .Include(x => x.Effects).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                // .Include(x => x.Effects).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .ToListAsync();

            if (characters == null) return null;

            characters.ForEach(x => x.Active = false);
            var activeChar = (await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier))?.ActiveCharacter;
            if (activeChar != null)
            {
                var match = characters.FirstOrDefault(x => x.Id.Equals(activeChar.Id));
                if (match != null)
                    match.Active = true;
            }

            return characters;
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            // If the character does not exist in the database, abort
            if (await _context.Characters.CountAsync(c => c.Id.Equals(character.Id)) <= 0)
                return;

            _context.Update(character);

            if (character.Active)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                if (user != null)
                {
                    user.ActiveCharacter = character;
                    _context.Update(user);
                }
                else
                {
                    await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = character });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task LoadStatistics(Character character)
        {
            await _context.Entry(character).Collection(x => x.Statistics).Query()
                .Include(x => x.Statistic)
                .Include(x => x.StatisticValue)
                .LoadAsync();
        }

        public async Task LoadEffects(Character character)
        {
            await _context.Entry(character).Collection(x => x.Effects).Query()
                .Include(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .LoadAsync();
        }
    }
}

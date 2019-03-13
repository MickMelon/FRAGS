using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Frags.Database.Repositories;
using Raven.Client.Documents;

namespace Frags.Database.DataAccess
{
    public class RavenDbCharacterProvider : ICharacterProvider
    {
        private readonly IDocumentStore _store;

        private readonly IMapper _mapper;

        public RavenDbCharacterProvider(IDocumentStore store)
        {
            _store = store;
            
            _mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Character, CharacterDto>()));
        }

        private async Task<Character> CreateCharacterAsync(Character character)
        {
            var charDto = _mapper.Map<CharacterDto>(character);

            using (var session = _store.OpenAsyncSession())
            {
                // Check the database for a character with the same ID as the new one
                // If one exists, don't add it
                if (await session.Query<CharacterDto>().CountAsync(x => x.Id == charDto.Id) > 0)
                    return null;

                // Add the new character to the database
                await session.StoreAsync(charDto);

                // Find a matching user object with the character
                var user = await session.Query<User>().FirstOrDefaultAsync(usr => usr.UserIdentifier == character.UserIdentifier);

                if (user == null)
                {
                    // Create a User object if one doesn't already exist (we'll set their only character as Active)
                    await session.StoreAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
                }
                else if (character.Active)
                {
                    // If the character is Active, match it on the User object
                    user.ActiveCharacter = charDto;
                }

                await session.SaveChangesAsync();
                return _mapper.Map<Character>(charDto);
            }
        }

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(ulong userIdentifier, string name) =>
            await CreateCharacterAsync(new Character(userIdentifier, name));

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(string id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "") =>
            await CreateCharacterAsync(new Character(id, userIdentifier, active, name, description, story));

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var user = await session.Query<User>().Where(c => c.UserIdentifier == userIdentifier).FirstOrDefaultAsync();
                if (user == null) return null;

                return _mapper.Map<Character>(user.ActiveCharacter);
            }
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var charDtos = await session.Query<CharacterDto>().Where(c => c.UserIdentifier == userIdentifier).ToListAsync();
                return _mapper.Map<List<Character>>(charDtos);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            using (var session = _store.OpenAsyncSession())
            {
                // Search the database for a CharacterDto matching "character's" ID
                var dbChar = await session.Query<CharacterDto>().Where(c => c.Id.Equals(character.Id)).FirstOrDefaultAsync();
                // If it doesn't exist, abort
                if (dbChar == null)
                    return;

                // Replace dbChar with a mapped version of "character" to overwrite later
                dbChar = _mapper.Map<CharacterDto>(character);

                // We only need to update the User object if a different character becomes Active
                if (character.Active)
                {
                    // Find a matching user object with the character
                    var user = await session.Query<User>().FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                    // User object exists
                    if (user != null)
                    {
                        // Set updated character as Active
                        user.ActiveCharacter = dbChar;
                    }
                    else
                    {
                        // Make a new User object (this shouldn't really happen since CreateCharacterAsync will generate a User object)
                        await session.StoreAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                    }
                }

                await session.SaveChangesAsync();
            }
        }
    }
}
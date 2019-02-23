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

        private async Task<CharacterDto> CreateCharacterAsync(Character character)
        {
            var charDto = _mapper.Map<CharacterDto>(character);

            using (var session = _store.OpenAsyncSession())
            {
                await session.StoreAsync(charDto);

                var user = await session.Query<User>().FirstOrDefaultAsync(usr => usr.UserIdentifier == character.UserIdentifier);

                if (user == null)
                {
                    await session.StoreAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
                }
                else if (character.Active)
                {
                    user.ActiveCharacter = charDto;
                }

                await session.SaveChangesAsync();
                return charDto;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CreateCharacterAsync(ulong userIdentifier, string name)
        {
            await CreateCharacterAsync(new Character(userIdentifier, name));
            return true;
        }

        public async Task<bool> CreateCharacterAsync(int id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "")
        {
            await CreateCharacterAsync(new Character(id, userIdentifier, active, name, description, story));
            return true;
        }

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
                var dbChar = await session.Query<CharacterDto>().Where(c => c.Equals(character)).FirstOrDefaultAsync();
                if (dbChar == null) return;

                dbChar = _mapper.Map<CharacterDto>(character);

                if (character.Active)
                {
                    var active = await session.Query<User>().FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                    if (active != null)
                    {
                        active.ActiveCharacter = dbChar;
                        await session.StoreAsync(active);
                    }
                    else
                    {
                        await session.StoreAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                    }
                }

                await session.StoreAsync(dbChar);
                await session.SaveChangesAsync();
            }
        }
    }
}
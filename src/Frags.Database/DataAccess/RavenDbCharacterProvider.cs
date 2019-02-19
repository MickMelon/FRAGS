using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Frags.Database.Repositories;
using Raven.Client.Documents;
//using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class RavenDbCharacterProvider : ICharacterProvider
    {
        private readonly RavenDbRepository<User> _activeRepo;
        private readonly RavenDbRepository<CharacterDto> _charRepo;

        private readonly IMapper _mapper;

        public RavenDbCharacterProvider(RavenDbRepository<User> activeRepo, RavenDbRepository<CharacterDto> charRepo)
        {
            _activeRepo = activeRepo;
            _charRepo = charRepo;
            
            _mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Character, CharacterDto>()));
        }

        private async Task<CharacterDto> CreateCharacterAsync(Character character)
        {
            var charDto = _mapper.Map<CharacterDto>(character);
            await _charRepo.AddAsync(charDto);

            if (character.Active)
            {
                var active = await _activeRepo.Query.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                if (active == null)
                    await _activeRepo.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
            }

            return charDto;
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
            var active = await _activeRepo.Query.Where(c => c.UserIdentifier == userIdentifier).FirstOrDefaultAsync();
            if (active == null) return null;

            return _mapper.Map<Character>(active.ActiveCharacter);
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            var charDtos = await _charRepo.Query.Where(c => c.UserIdentifier == userIdentifier).ToListAsync();
            return _mapper.Map<List<Character>>(charDtos);
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            var dbChar = await _charRepo.Query.Where(c => c.Equals(character)).FirstOrDefaultAsync();
            if (dbChar == null) return;
            
            dbChar = _mapper.Map<CharacterDto>(character);

            if (character.Active)
            {
                var active = await _activeRepo.Query.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);
                
                if (active != null)
                {
                    active.ActiveCharacter = dbChar;
                    await _activeRepo.SaveAsync(active);
                }
                else
                {
                    await _activeRepo.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                }
            }

            await _charRepo.SaveAsync(dbChar);
        }
    }
}
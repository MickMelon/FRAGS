using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Frags.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class RepositoryCharacterProvider : ICharacterProvider
    {
        private readonly IRepository<CharacterDto> _charRepo;
        private readonly IMapper _mapper;

        public RepositoryCharacterProvider(IRepository<CharacterDto> charRepo)
        {
            _charRepo = charRepo;
            
            _mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Character, CharacterDto>()));
        }

        /// <inheritdoc/>
        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            var character = new Character((int)discordId, name);
            var charDto = _mapper.Map<CharacterDto>(character);

            await _charRepo.AddAsync(charDto);
            return true;
        }

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong discordId)
        {
            var charDto = await _charRepo.Query.Where(c => c.Id == (int)discordId).FirstOrDefaultAsync();
            return _mapper.Map<Character>(charDto);          
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            var charDtos = await _charRepo.Query.Where(c => c.Id == (int)discordId).ToListAsync();
            return _mapper.Map<List<Character>>(charDtos);
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            var dbChar = await _charRepo.Query.Where(c => c.Id == (int)character.Id).FirstOrDefaultAsync();
            if (dbChar == null) return;
            
            dbChar = _mapper.Map<CharacterDto>(character);
            await _charRepo.SaveAsync(dbChar);
        }
    }
}
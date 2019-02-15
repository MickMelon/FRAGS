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
    /**
     * The real implementation of this class would be
     * contained in the Frags.Database project.    
     */
    /// <inheritdoc/>
    public class EfCharacterProvider : ICharacterProvider
    {
        private IRepository<CharacterDto> _charRepo;
        private IMapper _mapper;

        public EfCharacterProvider(IMapper mapper, IRepository<CharacterDto> charRepo)
        {
            _mapper = mapper;
            _charRepo = charRepo;
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
            //await Task.Delay(0); // just to ignore warning
            var charDto = await _charRepo.Query.Where(c => c.Id == (int)discordId).FirstOrDefaultAsync();
            return _mapper.Map<Character>(charDto);          
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            // await Task.Delay(0); // just to ignore warning
            // return _characters.Where(c => c.Id == (int)discordId).ToList();
            return await Task.FromResult(new List<Character>());
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            // var dbChar = _characters.Where(c => c.Id == (int)character.Id).FirstOrDefault();
            // if (dbChar == null) return;
            
            // dbChar = character;
            // await Task.Delay(0);            
        }
    }
}
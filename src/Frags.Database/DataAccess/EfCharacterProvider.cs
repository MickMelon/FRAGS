using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Frags.Database.Repositories;
using Frags.Database.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<CharacterDto> _charRepo;

        private readonly IMapper _mapper;

        public EfCharacterProvider(IRepository<User> userRepo, IRepository<CharacterDto> charRepo)
        {
            _userRepo = userRepo;
            _charRepo = charRepo;
            
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Character, CharacterDto>()
                    .ForMember(dto => dto.StatisticMappings, opt => opt.MapFrom<StatDictionaryToList>());
                cfg.CreateMap<CharacterDto, Character>()
                    .ForMember(poco => poco.Statistics, opt => opt.MapFrom<StatListToDictionary>());
            });

            _mapper = new Mapper(mapperConfig);
        }

        private async Task<Character> CreateCharacterAsync(Character character)
        {
            var charDto = _mapper.Map<CharacterDto>(character);

            // Check the database for a character with the same ID as the new one
            // If one exists, don't add it
            if (await _charRepo.Query.CountAsync(x => x.Id == charDto.Id) > 0)
                return null;

            await _charRepo.AddAsync(charDto);

            if (character.Active)
            {
                var user = await _userRepo.Query.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                if (user == null)
                    await _userRepo.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
            }

            return _mapper.Map<Character>(charDto);
        }

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(ulong userIdentifier, string name) =>
            await CreateCharacterAsync(new Character(userIdentifier, name));

        public async Task<Character> CreateCharacterAsync(string id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "") =>
            await CreateCharacterAsync(new Character(id, userIdentifier, active, name, description, story));

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            var user = await _userRepo.Query.Where(c => c.UserIdentifier == userIdentifier).FirstOrDefaultAsync();
            if (user == null) return null;

            return _mapper.Map<Character>(user.ActiveCharacter);
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
            // If the character does not exist in the database, abort
            if (await _charRepo.Query.Where(c => c.Id.Equals(character.Id)).CountAsync() <= 0)
                return;
            
            var dbChar = _mapper.Map<CharacterDto>(character);
            await _charRepo.SaveAsync(dbChar);

            if (character.Active)
            {
                var user = await _userRepo.Query.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);
                
                if (user != null)
                {
                    user.ActiveCharacter = dbChar;
                    await _userRepo.SaveAsync(user);
                }
                else
                {
                    await _userRepo.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                }
            }            
        }
    }
}
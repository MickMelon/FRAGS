using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Frags.Database.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        public EfCharacterProvider(RpgContext context)
        {
            _context = context;
            
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
            if (await _context.Characters.CountAsync(x => x.Id.Equals(charDto.Id)) > 0)
                return null;

            await _context.AddAsync(charDto);

            if (character.Active)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

                if (user == null)
                    await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<Character>(charDto);
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
            var character = await _context.Users.Where(c => c.UserIdentifier == userIdentifier)
                .Select(usr => usr.ActiveCharacter)
                    .Include(charDto => charDto.StatisticMappings).ThenInclude(statMap => statMap.Statistic)
                    .Include(charDto => charDto.StatisticMappings).ThenInclude(statMap => statMap.StatisticValue)
                .FirstOrDefaultAsync();
            
            if (character == null) return null;

            return _mapper.Map<Character>(character);
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            var charDtos = await _context.Characters.Where(c => c.UserIdentifier == userIdentifier)
                .Include(charDto => charDto.StatisticMappings).ThenInclude(statMap => statMap.Statistic)
                .Include(charDto => charDto.StatisticMappings).ThenInclude(statMap => statMap.StatisticValue)
                .ToListAsync();
                
            return _mapper.Map<List<Character>>(charDtos);
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            // If the character does not exist in the database, abort
            if (await _context.Characters.CountAsync(c => c.Id.Equals(character.Id)) <= 0)
                return;
            
            var dbChar = _mapper.Map<CharacterDto>(character);
            _context.Update(dbChar);

            if (character.Active)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);
                
                if (user != null)
                {
                    user.ActiveCharacter = dbChar;
                    _context.Update(user);
                }
                else
                {
                    await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
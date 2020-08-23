using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Core.Common;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Frags.Database.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        private readonly IUserProvider _userProvider;

        public EfCharacterProvider(RpgContext context, IMapper mapper, IUserProvider userProvider)
        {
            _context = context;

            _mapper = mapper;

            _userProvider = userProvider;
        }

        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == discordId);

            if (userDto == null)
            {
                await _userProvider.CreateUserAsync(discordId);
                userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == discordId);
            }

            var charDto = _mapper.Map<CharacterDto>(new Character(_mapper.Map<User>(userDto), name));
            await _context.AddAsync(charDto);

            //userDto.Characters.Add(charDto);
            userDto.ActiveCharacter = charDto;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            // Guess we still need the holy water, damn.
            var dto = await _context.Users.Where(c => c.UserIdentifier == userIdentifier)
                .Select(usr => usr.ActiveCharacter)
                .Include(x => x.User)
                .Include(x => x.Campaign)
                .Include(x => x.Statistics).ThenInclude(x => x.Statistic)
                .Include(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .FirstOrDefaultAsync();
            
            if (dto == null) return null;
            dto.Active = true;

            var mapped = _mapper.Map<Character>(dto);

            // TODO: move this to automapper configuration
            // Convert from StatisticMapping list to Dictionary
            mapped.Statistics.Clear();
            foreach (var stat in dto.Statistics)
                mapped.Statistics.Add(_mapper.Map<Statistic>(stat.Statistic), stat.StatisticValue);

            mapped.Effects = _mapper.Map<List<Effect>>(dto.EffectMappings.Select(x => x.Effect).ToList());
            return mapped;
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            ICollection<CharacterDto> charDtos = (await _context.Users.Where(c => c.UserIdentifier == userIdentifier)
                .Include(x => x.Characters).ThenInclude(x => x.User)
                .Include(x => x.Characters).ThenInclude(x => x.Campaign)
                .Include(x => x.Characters).ThenInclude(x => x.Statistics).ThenInclude(x => x.Statistic)
                .Include(x => x.Characters).ThenInclude(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                .Include(x => x.Characters).ThenInclude(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.Characters).ThenInclude(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .FirstOrDefaultAsync())?.Characters;

            if (charDtos == null) return null;


            // This sets CharacterDto.Active properly
            foreach (var x in charDtos) x.Active = false;
            var activeChar = (await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier))?.ActiveCharacter;
            if (activeChar != null)
            {
                var match = charDtos.FirstOrDefault(x => x.Id.Equals(activeChar.Id));
                if (match != null)
                    match.Active = true;
            }

            var mappedList = new List<Character>();
            foreach (var dto in charDtos)
            {
                var mapped = _mapper.Map<Character>(dto);
                mapped.Effects = _mapper.Map<List<Effect>>(dto.EffectMappings.Select(x => x.Effect).ToList());

                // Convert from StatisticMapping list to Dictionary
                mapped.Statistics.Clear();
                foreach (var stat in dto.Statistics)
                    mapped.Statistics.Add(_mapper.Map<Statistic>(stat.Statistic), stat.StatisticValue);

                mappedList.Add(mapped);
            }

            return mappedList;
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            // If the character does not exist in the database, abort
            if (await _context.Characters.CountAsync(c => c.Id.Equals(character.Id)) <= 0)
                return;
            
            var dbChar = await _context.Characters.Where(x => x.Id.Equals(character.Id))
                .Include(x => x.Statistics).ThenInclude(x => x.Statistic)
                .Include(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .FirstOrDefaultAsync();

            _mapper.Map<Character, CharacterDto>(character, dbChar);

            if (dbChar.EffectMappings == null)
                dbChar.EffectMappings = new List<EffectMapping>();
            else
                dbChar.EffectMappings.Clear();

            foreach (var effect in character.Effects)
            {
                var effectDto = _mapper.Map<EffectDto>(effect);
                dbChar.EffectMappings.Add(new EffectMapping { Effect = effectDto, Character = dbChar });
            }

            // Convert from Statistic Dictionary to StatisticMapping List
            dbChar.Statistics.Clear();
            foreach (var stat in character.Statistics)
                dbChar.Statistics.Add(new StatisticMapping(_mapper.Map<StatisticDto>(stat.Key), stat.Value));

            _context.Update(dbChar);

            if (character.Active)
            {
                var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.User.UserIdentifier);
                
                if (userDto != null)
                {
                    userDto.ActiveCharacter = dbChar;
                    _context.Update(userDto);
                }
                else
                {
                    await _context.AddAsync(new UserDto(character.User.UserIdentifier, dbChar));
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
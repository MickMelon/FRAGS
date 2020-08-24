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
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return _mapper.Map<Character>(dto);
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
                .AsNoTracking()
                .FirstOrDefaultAsync())?.Characters;

            return _mapper.Map<List<Character>>(charDtos);
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            CharacterDto dto = _mapper.Map<CharacterDto>(character);
            
            _context.Update(dto);
            await _context.SaveChangesAsync();
        }
    }
}
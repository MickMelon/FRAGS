using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.DataAccess;
using Frags.Database.Characters;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfUserProvider : IUserProvider
    {
        private readonly RpgContext _context;
        private readonly IMapper _mapper;

        public EfUserProvider(RpgContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> CreateUserAsync(ulong userId)
        {
            var dto = new UserDto(userId);
            
            await _context.AddAsync(dto);
            await _context.SaveChangesAsync();

            return _mapper.Map<User>(dto);
        }

        public async Task<User> GetUserAsync(ulong userId)
        {
            return _mapper.Map<User>(await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userId));
        }

        public async Task UpdateUserAsync(User user)
        {
            UserDto userDto = _context.Users.Find(user.Id);
            CharacterDto charDto = await _context.Characters.FirstOrDefaultAsync(x => x.Id == user.ActiveCharacter.Id);
            userDto.ActiveCharacter = charDto;

            _context.Update(userDto);
            await _context.SaveChangesAsync();
        }
    }
}
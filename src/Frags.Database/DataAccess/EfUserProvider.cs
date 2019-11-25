using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.DataAccess;
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
            var user = new User(userId);
            
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserAsync(ulong userId)
        {
            return _mapper.Map<User>(await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userId));
        }

        public async Task UpdateUserAsync(User user)
        {
            var dto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == user.UserIdentifier);
            _mapper.Map(user, dto);
            _context.Update(dto);
            await _context.SaveChangesAsync();
        }
    }
}
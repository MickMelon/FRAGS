using System.Threading.Tasks;

using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfUserProvider : IUserProvider
    {
        private readonly RpgContext _context;

        public EfUserProvider(RpgContext context)
        {
            _context = context;
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
            return await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
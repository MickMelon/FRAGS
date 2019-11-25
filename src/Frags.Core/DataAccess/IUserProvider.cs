using System.Threading.Tasks;
using Frags.Core.Common;

namespace Frags.Core.DataAccess
{
    public interface IUserProvider
    {
        Task<User> CreateUserAsync(ulong userId);

        Task<User> GetUserAsync(ulong userId);

        Task UpdateUserAsync(User user);
    }
}
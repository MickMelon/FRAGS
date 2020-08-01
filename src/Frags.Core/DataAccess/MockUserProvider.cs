using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Common;

namespace Frags.Core.DataAccess
{
    public class MockUserProvider : IUserProvider
    {
        private List<User> _users = new List<User>();

        public Task<User> CreateUserAsync(ulong userId)
        {
            var user = new User(userId);
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> GetUserAsync(ulong userId)
        {
            return Task.FromResult(_users.Find(x => x.UserIdentifier == userId));
        }

        public Task UpdateUserAsync(User user)
        {
            _users[_users.FindIndex(x => x.UserIdentifier == user.UserIdentifier)] = user;
            return Task.CompletedTask;
        }
    }
}
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Database;
using Frags.Database.Characters;
using Frags.Database.DataAccess;
using Frags.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Database.DataAccess
{
    public class RepositoryCharacterProviderTests
    {
        #region Character Creation Tests
        [Fact]
        public async Task CreateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            var efRepo = new EfThreadSafeRepository<CharacterDto>(context);
            var actRepo = new EfThreadSafeRepository<ActiveCharacter>(context);
            
            var provider = new RepositoryCharacterProvider(actRepo, efRepo);

            await provider.CreateCharacterAsync(1, 305847674974896128, true, "Melon Head");
            var result = await provider.GetActiveCharacterAsync(305847674974896128);

            Assert.True(result.UserIdentifier == 305847674974896128);
        }
        #endregion
    }
}
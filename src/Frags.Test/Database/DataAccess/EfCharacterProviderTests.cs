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
    public class EfCharacterProviderTests
    {
        #region Character Creation Tests
        [Fact]
        public async Task EntityFramework_CreateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            var efRepo = new EfThreadSafeRepository<CharacterDto>(context);
            var actRepo = new EfThreadSafeRepository<User>(context);
            
            var provider = new EfCharacterProvider(actRepo, efRepo);

            await provider.CreateCharacterAsync("1", 305847674974896128, true, "Melon Head");
            var result = await provider.GetActiveCharacterAsync(305847674974896128);

            Assert.True(result.UserIdentifier == 305847674974896128);
        }

        [Fact]
        public async Task EntityFramework_UpdateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").EnableSensitiveDataLogging().Options);
            var efRepo = new EfThreadSafeRepository<CharacterDto>(context);
            var actRepo = new EfThreadSafeRepository<User>(context);
            
            var provider = new EfCharacterProvider(actRepo, efRepo);

            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", newName = "Mr. Melon", id = "1";

            await provider.CreateCharacterAsync(id, userIdentifier, true, oldName);

            var result = await provider.GetActiveCharacterAsync(userIdentifier);

            // Simulate transient dependencies (will fail without this)
            context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").EnableSensitiveDataLogging().Options);
            efRepo = new EfThreadSafeRepository<CharacterDto>(context);
            actRepo = new EfThreadSafeRepository<User>(context);
            provider = new EfCharacterProvider(actRepo, efRepo);

            result.Name = newName;
            await provider.UpdateCharacterAsync(result);

            result = await provider.GetActiveCharacterAsync(userIdentifier);
            Assert.Equal(newName, result.Name);
        }
        #endregion
    }
}
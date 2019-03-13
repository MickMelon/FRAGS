using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
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
        private readonly ITestOutputHelper _output;

        public EfCharacterProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
        public async Task EntityFramework_CharacterStatistics_EntityMatchesInput()
        {
            var deps = ReloadDependencies();

            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", id = "1";
            var strength = await deps.statProvider.CreateAttributeAsync("Strength");
            var value = new StatisticValue(5);

            await deps.charProvider.CreateCharacterAsync(id, userIdentifier, true, oldName);

            var result = await deps.charProvider.GetActiveCharacterAsync(userIdentifier);

            // Simulate transient dependencies (will fail without this)
            deps = ReloadDependencies();

            result.Statistics = new Dictionary<Statistic, StatisticValue>{ { strength, value } };
            await deps.charProvider.UpdateCharacterAsync(result);

            result = await deps.charProvider.GetActiveCharacterAsync(userIdentifier);
            Assert.True(result.Statistics.Count > 0);
        }

        [Fact]
        public async Task EntityFramework_UpdateCharacter_EntityMatchesInput()
        {
            var deps = ReloadDependencies();

            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", newName = "Mr. Melon", id = "1";

            await deps.charProvider.CreateCharacterAsync(id, userIdentifier, true, oldName);

            var result = await deps.charProvider.GetActiveCharacterAsync(userIdentifier);

            // Simulate transient dependencies (will fail without this)
            deps = ReloadDependencies();

            result.Name = newName;
            await deps.charProvider.UpdateCharacterAsync(result);

            result = await deps.charProvider.GetActiveCharacterAsync(userIdentifier);
            Assert.Equal(newName, result.Name);
        }

        private (RpgContext context, EfThreadSafeRepository<CharacterDto> charRepo, EfThreadSafeRepository<User> userRepo, EfThreadSafeRepository<Statistic> statRepo, EfCharacterProvider charProvider, EfStatisticProvider statProvider) ReloadDependencies()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").EnableSensitiveDataLogging().Options);
            var charRepo = new EfThreadSafeRepository<CharacterDto>(context);
            var userRepo = new EfThreadSafeRepository<User>(context);
            var statRepo = new EfThreadSafeRepository<Statistic>(context);
            return (context,
            charRepo,
            userRepo,
            statRepo,
            new EfCharacterProvider(userRepo, charRepo),
            new EfStatisticProvider(statRepo));
        }
        #endregion
    }
}
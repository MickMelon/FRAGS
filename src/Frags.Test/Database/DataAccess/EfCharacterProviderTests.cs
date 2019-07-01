using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.Characters;
using Frags.Database.DataAccess;
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
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CreateCharacter_EntityMatchesInput"
            });
            
            var provider = new EfCharacterProvider(context);

            await provider.CreateCharacterAsync(1, 305847674974896128, true, "Melon Head");
            var result = await provider.GetActiveCharacterAsync(305847674974896128);

            Assert.True(result.UserIdentifier == 305847674974896128);
        }

        [Fact]
        public async Task EntityFramework_CharacterStatistics_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CharacterStatistics_EntityMatchesInput"
            });

            var provider = new EfCharacterProvider(context);
            var statProvider = new EfStatisticProvider(context);

            ulong userIdentifier = 305847674974896128;
            string name = "Melon Head";
            int id = 1;
            
            var strength = await statProvider.CreateAttributeAsync("Strength");
            var value = new StatisticValue(5);
            await provider.CreateCharacterAsync(id, userIdentifier, true, name);
            var result = await provider.GetActiveCharacterAsync(userIdentifier);

            result.SetStatistic(strength, value);
            await provider.UpdateCharacterAsync(result);

            result = await provider.GetActiveCharacterAsync(userIdentifier);
            Assert.True(result.Statistics.Count > 0);
        }

        [Fact]
        public async Task EntityFramework_UpdateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "UpdateCharacter_EntityMatchesInput"
            });

            var provider = new EfCharacterProvider(context);
            
            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", newName = "Mr. Melon";
            int id = 1;

            var result = await provider.CreateCharacterAsync(id, userIdentifier, true, oldName);

            result.Name = newName;
            await provider.UpdateCharacterAsync(result);

            result = await provider.GetActiveCharacterAsync(userIdentifier);
            Assert.Equal(newName, result.Name);
        }
        #endregion
    }
}
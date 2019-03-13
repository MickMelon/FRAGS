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
        #region Character Creation Tests
        [Fact]
        public async Task EntityFramework_CreateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            var provider = new EfCharacterProvider(context);

            await provider.CreateCharacterAsync("1", 305847674974896128, true, "Melon Head");
            var result = await provider.GetActiveCharacterAsync(305847674974896128);

            Assert.True(result.UserIdentifier == 305847674974896128);
        }

        [Fact]
        public async Task EntityFramework_CharacterStatistics_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            var provider = new EfCharacterProvider(context);
            var statProvider = new EfStatisticProvider(context);

            ulong userIdentifier = 305847674974896128;
            string name = "Melon Head", id = "1";
            
            var strength = await statProvider.CreateAttributeAsync("Strength");
            var value = new StatisticValue(5);
            await provider.CreateCharacterAsync(id, userIdentifier, true, name);
            var result = await provider.GetActiveCharacterAsync(userIdentifier);

            // Simulate transient dependencies (will fail without this)
            context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            provider = new EfCharacterProvider(context);
            statProvider = new EfStatisticProvider(context);

            result.Statistics.Add(strength, value);
            await provider.UpdateCharacterAsync(result);

            result = await provider.GetActiveCharacterAsync(userIdentifier);
            Assert.True(result.Statistics.Count > 0);
        }

        [Fact]
        public async Task EntityFramework_UpdateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            var provider = new EfCharacterProvider(context);
            
            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", newName = "Mr. Melon", id = "1";

            var result = await provider.CreateCharacterAsync(id, userIdentifier, true, oldName);
            context.Dispose();

            // Simulate transient dependencies (will fail without this)
            context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb").Options);
            provider = new EfCharacterProvider(context);

            result.Name = newName;
            await provider.UpdateCharacterAsync(result);

            result = await provider.GetActiveCharacterAsync(userIdentifier);
            Assert.Equal(newName, result.Name);
        }
        #endregion
    }
}
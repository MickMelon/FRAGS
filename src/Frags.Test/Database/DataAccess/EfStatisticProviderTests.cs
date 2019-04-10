using System.Collections.Generic;
using System.Linq;
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
    public class EfStatisticProviderTests
    {
        #region Statistic Creation Tests
        [Fact]
        public async Task EntityFramework_CreateStatistic_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("CreateStatistic_EntityMatchesInput").Options);
            var provider = new EfStatisticProvider(context);

            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            Assert.Equal("Strength", result.Name);
        }

        [Fact]
        public async Task EntityFramework_GetAllStatistics_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("GetAllStatistics_EntityMatchesInput").Options);
            var statProvider = new EfStatisticProvider(context);

            await statProvider.CreateAttributeAsync("Strength");
            await statProvider.CreateSkillAsync("Powerlifting", "Strength");

            var result = await statProvider.GetAllStatisticsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task EntityFramework_UpdateStatistic_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("UpdateStatistic_EntityMatchesInput").Options);
            var provider = new EfStatisticProvider(context);
            
            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            result.Name = "STR";
            await provider.UpdateStatisticAsync(result);

            Assert.NotNull(await provider.GetStatisticAsync("STR"));
        }
        #endregion

        #region Statistic Deletion Tests
        [Fact]
        public async Task DeleteStatistic_ValidInput_CharacterNoLongerHasValue()
        {
            // Arrange
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("DeleteStatistic_ValidInput_CharacterNoLongerHasValue").Options);
            var provider = new EfStatisticProvider(context);
            var charProvider = new EfCharacterProvider(context);

            var strength = await provider.CreateAttributeAsync("strength");
            await charProvider.CreateCharacterAsync(1, "bob");

            var bob = await charProvider.GetActiveCharacterAsync(1);
            bob.SetStatistic(strength, new StatisticValue(5));

            // Act
            bool valueDidExist = bob.GetStatistic(strength) is StatisticValue;
            await provider.DeleteStatisticAsync(strength);
            bob = await charProvider.GetActiveCharacterAsync(1);
            bool valueNoLongerExists = bob.GetStatistic(strength) is null;

            // Assert
            Assert.True(valueDidExist && valueNoLongerExists);
        }
        #endregion
    }
}
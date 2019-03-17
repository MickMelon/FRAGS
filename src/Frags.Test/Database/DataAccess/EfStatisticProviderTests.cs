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
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb1").Options);
            var provider = new EfStatisticProvider(context);

            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            Assert.Equal("Strength", result.Name);
        }

        [Fact]
        public async Task EntityFramework_GetAllStatistics_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb2").Options);
            var statProvider = new EfStatisticProvider(context);

            await statProvider.CreateAttributeAsync("Strength");
            await statProvider.CreateSkillAsync("Powerlifting", "Strength");

            var result = await statProvider.GetAllStatisticsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task EntityFramework_UpdateStatistic_EntityMatchesInput()
        {
            var context = new RpgContext(new DbContextOptionsBuilder<RpgContext>().UseInMemoryDatabase("TestDb3").Options);
            var provider = new EfStatisticProvider(context);
            
            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            result.Name = "STR";
            await provider.UpdateStatisticAsync(result);

            Assert.NotNull(await provider.GetStatisticAsync("STR"));
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.AutoMapper;
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
        public async Task CreateStatistic_ValidInput_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CreateStatistic_EntityMatchesInput"
            });

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);
            var provider = new EfStatisticProvider(context, mapper);

            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            Assert.Equal("Strength", result.Name);
        }

        [Fact]
        public async Task GetAllStatistics_ValidInput_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "GetAllStatistics_EntityMatchesInput"
            });

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);
            var statProvider = new EfStatisticProvider(context, mapper);

            await statProvider.CreateAttributeAsync("Strength");
            await statProvider.CreateSkillAsync("Powerlifting", "Strength");

            var result = await statProvider.GetAllStatisticsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateStatistic_ValidInput_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "UpdateStatistic_EntityMatchesInput"
            });

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);
            var provider = new EfStatisticProvider(context, mapper);
            
            await provider.CreateAttributeAsync("Strength");
            var result = await provider.GetStatisticAsync("Strength");

            result.Name = "STR";
            result.Aliases = "STR" + "/";
            await provider.UpdateStatisticAsync(result);

            Assert.NotNull(await provider.GetStatisticAsync("STR"));
        }
        #endregion

        #region Statistic Deletion Tests
        [Fact]
        public async Task DeleteStatistic_ValidInput_CharacterNoLongerHasValue()
        {
            // Arrange
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "DeleteStatistic_ValidInput_CharacterNoLongerHasValue"
            });

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);
            var provider = new EfStatisticProvider(context, mapper);
            var userProvider = new EfUserProvider(context, mapper);
            var charProvider = new EfCharacterProvider(context, mapper, userProvider);

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

        #region Campaign Statistic Tests
        [Fact]
        public async Task GetStatisticFromCampaignAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var genOptions = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "GetStatisticFromCampaignAsync_ValidInput_ReturnSuccess"
            };

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);

            // Simulate injected DbContext and dependencies with Scoped lifetime (One instance per "request", i.e. a command)
            using (var context = new RpgContext(genOptions))
            {
                var campProvider = new EfCampaignProvider(context, mapper, null);
                await campProvider.CreateCampaignAsync(1, "campaign");
            }

            using (var context = new RpgContext(genOptions))
            {
                var provider = new EfStatisticProvider(context, mapper);
                var userProvider = new EfUserProvider(context, mapper);
                var campProvider = new EfCampaignProvider(context, mapper, null);

                Campaign campaign = await campProvider.GetCampaignAsync("campaign");
                await provider.CreateAttributeAsync("strength", campaign);
            }

            using (var context = new RpgContext(genOptions))
            {
                var provider = new EfStatisticProvider(context, mapper);
                var userProvider = new EfUserProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);

                await charProvider.CreateCharacterAsync(1, "bob");
            }

            // Act
            Character bob;
            Statistic strength;
            using (var context = new RpgContext(genOptions))
            {
                var statProvider = new EfStatisticProvider(context, mapper);
                var userProvider = new EfUserProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);
                var campProvider = new EfCampaignProvider(context, mapper, null);

                bob = await charProvider.GetActiveCharacterAsync(1);
                Campaign camp = await campProvider.GetCampaignAsync("campaign");
                strength = await statProvider.GetStatisticFromCampaignAsync("Strength", camp.Id);
                bob.SetStatistic(strength, new StatisticValue(5));
            }

            // Assert
            Assert.Equal(5, bob.GetStatistic(strength).Value);
        }
        #endregion
    }
}
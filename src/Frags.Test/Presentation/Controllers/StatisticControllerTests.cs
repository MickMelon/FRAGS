using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Presentation.Controllers
{
    public class StatisticControllerTests
    {
        private readonly ITestOutputHelper output;

        public StatisticControllerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region CreateAttribute & CreateStatistic Tests
        [Fact]
        public async Task CreateAttributeAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null, null);

            var result = await controller.CreateAttributeAsync("Wisdom");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateAttributeAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null, null);

            var result = await controller.CreateAttributeAsync("Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null, null);

            var result = await controller.CreateSkillAsync("Intimidation", "Strength");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null, null);

            var result = await controller.CreateSkillAsync("Powerlifting", "Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_InvalidAttributeName_ReturnCreationFailed()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null, null);

            var result = await controller.CreateSkillAsync("Intimidation", "STR");

            Assert.Equal(StatisticResult.StatisticCreationFailed(), result);
        }
        #endregion

        

        #region DeleteStatistic Tests
        
        [Fact]
        public async Task DeleteStatistic_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, new StatisticOptions()), null);

            // Act
            var result = await controller.DeleteStatisticAsync("strength");

            // Assert
            Assert.Equal(result, StatisticResult.StatisticDeletedSuccessfully());
        }

        [Fact]
        public async Task DeleteStatistic_InvalidStatName_ReturnNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, new StatisticOptions()), null);

            // Act
            var result = await controller.DeleteStatisticAsync("bacon");

            // Assert
            Assert.Equal(result, StatisticResult.StatisticNotFound());
        }
        #endregion
    }
}
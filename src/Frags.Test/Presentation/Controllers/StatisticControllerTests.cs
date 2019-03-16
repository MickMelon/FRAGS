using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
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

        #region 
        [Fact]
        public async Task SetAttributeAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialAttributesAtMax = 7
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);
            var character = await charProvider.GetActiveCharacterAsync(1);

            // Act
            await controller.SetAttributeAsync(1, "strength", 10);
            await controller.SetAttributeAsync(1, "perception", 2);
            await controller.SetAttributeAsync(1, "endurance", 6);
            await controller.SetAttributeAsync(1, "charisma", 6);
            await controller.SetAttributeAsync(1, "intelligence", 6);
            await controller.SetAttributeAsync(1, "agility", 5);
            await controller.SetAttributeAsync(1, "luck", 5);

            // Assert
            Statistic str = await statProvider.GetStatisticAsync("strength"),
            per = await statProvider.GetStatisticAsync("perception"),
            end = await statProvider.GetStatisticAsync("endurance"),
            cha = await statProvider.GetStatisticAsync("charisma"),
            inte = await statProvider.GetStatisticAsync("intelligence"),
            agi = await statProvider.GetStatisticAsync("agility"),
            lck = await statProvider.GetStatisticAsync("luck");

            Assert.True(character.Statistics[str].Value.Equals(10) &&
            character.Statistics[per].Value.Equals(2) &&
            character.Statistics[end].Value.Equals(6) &&
            character.Statistics[cha].Value.Equals(6) &&
            character.Statistics[inte].Value.Equals(6) && 
            character.Statistics[agi].Value.Equals(5) &&
            character.Statistics[lck].Value.Equals(5));
        }

        [Fact]
        public async Task SetAttributeAsync_NotEnoughPoints_ReturnNotEnoughPoints()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 10,
                InitialAttributePoints = 1
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(1, "luck", 2);

            // Assert
            Assert.Equal(GenericResult.NotEnoughPoints(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_AboveMaximum_ReturnTooHigh()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 10,
                InitialAttributePoints = 12,
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(1, "strength", 11);

            // Assert
            Assert.Equal(GenericResult.ValueTooHigh(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_BelowMinimum_ReturnTooLow()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 10
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(1, "strength", 0);

            // Assert
            Assert.Equal(GenericResult.ValueTooLow(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_TooManyAtMax_ReturnTooManyAtMax()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 2,
                InitialAttributePoints = 6,
                InitialAttributesAtMax = 2
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            await controller.SetAttributeAsync(1, "strength", 2);
            await controller.SetAttributeAsync(1, "perception", 2);
            var result = await controller.SetAttributeAsync(1, "endurance", 2);

            // Assert
            Assert.Equal(StatisticResult.TooManyAtMax(2), result);
        }

        [Fact]
        public async Task SetAttributeAsync_LevelTooHighButAttributesUnset_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var character = await charProvider.CreateCharacterAsync(100, "TooHigh");
            // the important bit
            character.Experience = 50000;

            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 10,
                InitialSetupMaxLevel = 1
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(100, "strength", 5);

            // Assert
            Assert.Equal(StatisticResult.StatisticSetSucessfully(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_LevelTooHigh_ReturnLevelTooHigh()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var character = await charProvider.CreateCharacterAsync(100, "TooHigh");

            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialSetupMaxLevel = 1
            };

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            await controller.SetAttributeAsync(100, "strength", 10);
            await controller.SetAttributeAsync(100, "perception", 2);
            await controller.SetAttributeAsync(100, "endurance", 6);
            await controller.SetAttributeAsync(100, "charisma", 6);
            await controller.SetAttributeAsync(100, "intelligence", 6);
            await controller.SetAttributeAsync(100, "agility", 5);
            await controller.SetAttributeAsync(100, "luck", 5);

            character.Experience = 50000;
            await charProvider.UpdateCharacterAsync(character);
            var result = await controller.SetAttributeAsync(100, "strength", 4);

            // Assert
            Assert.Equal(CharacterResult.LevelTooHigh(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_InvalidCharacterId_ReturnCharacterNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions();

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(ulong.MaxValue, "strength", 5);

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task SetAttributeAsync_InvalidStatisticName_ReturnStatisticNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions();

            var controller = new StatisticController(charProvider, statProvider, statOptions);

            // Act
            var result = await controller.SetAttributeAsync(1, "invalid", 5);

            // Assert
            Assert.Equal(StatisticResult.StatisticNotFound(), result);
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Xunit;

namespace Frags.Test.Presentation.Controllers
{
    public class RollControllerTests
    {
        #region RollStatisticAsync Tests
        [Fact]
        public async Task Roll_ValidValues_ReturnSuccess()
        {
            // Arrange
            var statProvider = new MockStatisticProvider();
            var strength = await statProvider.GetStatisticAsync("Strength");

            var provider = new MockCharacterProvider();
            var chars = await provider.GetAllCharactersAsync(1);

            // Give the new character a Statistic to test
            chars[0].Statistics = new Dictionary<Statistic, StatisticValue> { { strength, new StatisticValue(5) } };

            await provider.UpdateCharacterAsync(chars[0]);

            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAsync(1, "strength");

            // Assert
            Assert.True(result.GetType() == typeof(RollResult) && result.IsSuccess);
        }

        [Fact]
        public async Task Roll_InvalidStat_ReturnStatNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAsync(1, "invalid");

            // Assert
            Assert.True(StatisticResult.StatisticNotFound().Equals(result));
        }

        [Fact]
        public async Task Roll_InvalidStatValues_ReturnRollFailed()
        {
            // Arrange
            var statProvider = new MockStatisticProvider();
            var strength = await statProvider.GetStatisticAsync("Strength");

            var provider = new MockCharacterProvider();

            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            // Character should have null or empty Statistics list.
            var result = await controller.RollStatisticAsync(1, "strength");

            // Assert
            Assert.True(RollResult.RollFailed().Equals(result));
        }

        [Fact]
        public async Task Roll_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAsync(0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }
        #endregion

        #region RollStatisticAgainstAsync Tests
        [Fact]
        public async Task RollAgainst_ValidValues_ReturnSuccess()
        {
            // Arrange
            var statProvider = new MockStatisticProvider();
            var strength = await statProvider.GetStatisticAsync("Strength");

            var provider = new MockCharacterProvider();
            // Give characters statistics
            (await provider.GetAllCharactersAsync(1))[0].Statistics = new Dictionary<Statistic, StatisticValue>{ { strength, new StatisticValue(5) } };
            (await provider.GetAllCharactersAsync(2))[0].Statistics = new Dictionary<Statistic, StatisticValue>{ { strength, new StatisticValue(5) } };
            
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAgainstAsync(1, 2, "strength");

            // Assert
            Assert.True(result.GetType() == typeof(RollResult) && result.IsSuccess);
        }

        [Fact]
        public async Task RollAgainst_InvalidCaller_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAgainstAsync(0, 2, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task RollAgainst_InvalidTarget_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAgainstAsync(1, 0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task RollAgainst_InvalidStat_ReturnStatNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAgainstAsync(1, 2, "invalid");

            // Assert
            Assert.Equal(StatisticResult.StatisticNotFound(), result);
        }

        [Fact]
        public async Task RollAgainst_InvalidStatValues_ReturnRollFailed()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new RollController(provider, statProvider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollStatisticAgainstAsync(1, 2, "strength");

            // Assert
            Assert.Equal(RollResult.RollFailed(), result);
        }
        #endregion
    }
}
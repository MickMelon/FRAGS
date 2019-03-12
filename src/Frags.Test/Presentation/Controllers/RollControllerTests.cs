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
        private static readonly Attribute strength = new Attribute { Name = "Strength" };
        private static readonly StatisticValue value = new StatisticValue { Value = 5 };

        #region RollAsync Tests
        [Fact]
        public async Task Roll_ValidValues_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            await provider.CreateCharacterAsync(1, "bob");
            var chars = await provider.GetAllCharactersAsync(1);

            // Give the new character a Statistic to test
            chars[0].Statistics = new Dictionary<Statistic, StatisticValue> { { strength, value } };

            await provider.UpdateCharacterAsync(chars[0]);

            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAsync(1, "strength");

            // Assert
            Assert.True(result.GetType() == typeof(RollResult) && result.IsSuccess);
        }

        [Fact]
        public async Task Roll_InvalidSkill_ReturnSkillNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAsync(1, "invalid");

            // Assert
            Assert.True(SkillResult.SkillNotFound().Equals(result));
        }

        [Fact]
        public async Task Roll_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAsync(0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }
        #endregion

        #region RollAgainstAsync Tests
        [Fact]
        public async Task RollAgainst_ValidValues_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAgainstAsync(1, 2, "strength");

            // Assert
            Assert.True(result.GetType() == typeof(RollResult) && result.IsSuccess);
        }

        [Fact]
        public async Task RollAgainst_InvalidCaller_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAgainstAsync(0, 2, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task RollAgainst_InvalidTarget_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAgainstAsync(1, 0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task RollAgainst_InvalidSkill_ReturnSkillNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider, new RollOptions { RollMode = RollMode.Mock });

            // Act
            var result = await controller.RollAgainstAsync(1, 2, "invalid");

            // Assert
            Assert.Equal(SkillResult.SkillNotFound(), result);
        }
        #endregion
    }
}
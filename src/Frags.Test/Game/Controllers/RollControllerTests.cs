using System.Threading.Tasks;
using Frags.Core.DataAccess;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Xunit;

namespace Frags.Test.Game.Controllers
{
    public class RollControllerTests
    {
        #region RollAsync Tests
        [Fact]
        public async Task Roll_ValidValues_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAsync(1, "strength");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Roll_InvalidSkill_ReturnSkillNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAsync(1, "invalid");

            // Assert
            Assert.Equal(SkillResult.SkillNotFound().Message, result.Message);
        }

        [Fact]
        public async Task Roll_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAsync(0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound().Message, result.Message);
        }
        #endregion

        #region RollAgainstAsync Tests
        [Fact]
        public async Task RollAgainst_ValidValues_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAgainstAsync(1, 2, "strength");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RollAgainst_InvalidCaller_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAgainstAsync(0, 2, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound().Message, result.Message);
        }

        [Fact]
        public async Task RollAgainst_InvalidTarget_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAgainstAsync(1, 0, "strength");

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound().Message, result.Message);
        }

        [Fact]
        public async Task RollAgainst_InvalidSkill_ReturnSkillNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new RollController(provider);

            // Act
            var result = await controller.RollAgainstAsync(1, 2, "invalid");

            // Assert
            Assert.Equal(SkillResult.SkillNotFound().Message, result.Message);
        }
        #endregion
    }
}
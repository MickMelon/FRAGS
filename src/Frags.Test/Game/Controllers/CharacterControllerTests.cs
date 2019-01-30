using System.Threading.Tasks;
using Frags.Core.Controllers;
using Frags.Core.Controllers.Results;
using Frags.Core.DataAccess;
using Xunit;

namespace Frags.Test.Game.Controllers
{
    public class CharacterControllerTests
    {
        #region ShowCharacterAsync Tests
        [Fact]
        public async Task ShowCharacter_ValidId_Success()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.ShowCharacterAsync(1);

            // Assert
            //Assert.NotEqual(CharacterResult.CharacterNotFound(), result);
            // Compare CharacterResult
        }

        [Fact]
        public async Task ShowCharacter_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.ShowCharacterAsync(1);

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }
        #endregion

        #region RollAsync Tests
        [Fact]
        public async Task Roll_ValidIdValidSkill_Success()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.RollAsync(1, "lockpick");

            // Assert
            //Assert.Equal()
        }

        [Fact]
        public async Task Roll_ValidIdInvalidSkill_ReturnErrorResult()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.RollAsync(1, "dfg");

            // Assert
            Assert.Equal(GenericResult.Result("TODO"), result);
        }

        [Fact]
        public async Task Roll_InvalidId_ReturnError()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.RollAsync(1, "dfg");

            // Assert
            Assert.Equal(GenericResult.Result("TODO"), result);
        }
        #endregion
    }
}
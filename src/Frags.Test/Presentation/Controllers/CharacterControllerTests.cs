using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;
using Xunit;

namespace Frags.Test.Presentation.Controllers
{
    public class CharacterControllerTests
    {
        #region ShowCharacterAsync Tests
        [Fact]
        public async Task ShowCharacter_ValidId_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider, new StatisticOptions());
            var dbChar = await provider.GetActiveCharacterAsync(1);

            // Act
            var result = await controller.ShowCharacterAsync(1);
            var charResult = result as CharacterResult;
            var viewModel = charResult.ViewModel as ShowCharacterViewModel;

            // Assert
            Assert.True(CharacterResult.Show(dbChar).Equals(result) &&
                viewModel.Name.EqualsIgnoreCase(dbChar.Name) &&
                viewModel.Story.EqualsIgnoreCase(dbChar.Story) &&
                viewModel.Description.EqualsIgnoreCase(dbChar.Description));
        }

        [Fact]
        public async Task ShowCharacter_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider, new StatisticOptions());

            // Act
            var result = await controller.ShowCharacterAsync(0);

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }
        #endregion

        #region CreateCharacterAsync Tests
        [Fact]
        public async Task CreateCharacter_ValidValues_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider, new StatisticOptions());

            // Act
            var result = await controller.CreateCharacterAsync(1, "c");

            // Assert
            Assert.Equal(CharacterResult.CharacterCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateCharacter_PlayerHasExistingName_ReturnNameAlreadyExists()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider, new StatisticOptions());

            // Act
            var result = await controller.CreateCharacterAsync(1, "c1"); // Existing

            // Assert
            Assert.Equal(CharacterResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateCharacter_ExistingNameButNotBelongingToPlayer_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider, new StatisticOptions());

            // Act
            var result = await controller.CreateCharacterAsync(1, "c2"); // Existing

            // Assert
            Assert.Equal(CharacterResult.CharacterCreatedSuccessfully(), result);
        }
        #endregion
    }
}
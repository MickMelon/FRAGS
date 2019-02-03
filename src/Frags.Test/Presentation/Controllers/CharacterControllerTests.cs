using System.Threading.Tasks;
using Frags.Core.DataAccess;
using Frags.Core.Common.Extensions;
using Xunit;
using Frags.Core.Common;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;

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
            var controller = new CharacterController(provider);
            var dbChar = await provider.GetActiveCharacterAsync(1);

            // Act
            var result = await controller.ShowCharacterAsync(1);
            var charResult = result as CharacterResult;
            var viewModel = charResult.ViewModel as ShowCharacterViewModel;

            // Assert
            Assert.True(
                viewModel.Name.EqualsIgnoreCase(dbChar.Name) &&
                viewModel.Story.EqualsIgnoreCase(dbChar.Story) &&
                viewModel.Description.EqualsIgnoreCase(dbChar.Description));
        }

        [Fact]
        public async Task ShowCharacter_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            var controller = new CharacterController(provider);

            // Act
            var result = await controller.ShowCharacterAsync(0);

            // Assert
            Assert.Equal(Messages.CHAR_NOT_FOUND, result.Message);
        }
        #endregion
    }
}
using Frags.Core.Common.Attributes;
using Frags.Core.Controllers.ViewModels;
using Frags.Core.Models.Characters;
using Xunit;

namespace Frags.Test.Game.Common.Attributes
{
    public class ViewModelAttributeTests
    {
        [Fact]
        public void IsViewModel_ValidViewModel_ReturnTrue()
        {
            // Arrange
            var viewModel = new ShowCharacterViewModel();

            // Act
            bool result = ViewModelAttribute.IsViewModel(viewModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsViewModel_InvalidViewModel_ReturnFalse()
        {
            // Arrange
            var notViewModel = new Character(1, "C");

            // Act
            bool result = ViewModelAttribute.IsViewModel(notViewModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsViewModel_NullViewModel_ReturnFalse()
        {
            // Arrange
            object nullViewModel = null;

            // Act
            bool result = ViewModelAttribute.IsViewModel(nullViewModel);

            // Assert
            Assert.False(result);
        }
    }
}
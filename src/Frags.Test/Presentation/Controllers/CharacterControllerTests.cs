using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels.Characters;
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
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);
            var dbChar = await provider.GetActiveCharacterAsync(1);

            // Act
            var result = await controller.ShowCharacterAsync(1);
            var charResult = result as CharacterResult;
            var viewModel = charResult.ViewModel as ShowCharacterViewModel;

            // Assert
            Assert.True(CharacterResult.Show(dbChar, 1, "").Equals(result) &&
                viewModel.Name.EqualsIgnoreCase(dbChar.Name) &&
                viewModel.Story.EqualsIgnoreCase(dbChar.Story) &&
                viewModel.Description.EqualsIgnoreCase(dbChar.Description));
        }

        [Fact]
        public async Task ShowCharacter_InvalidId_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

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
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

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
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

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
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.CreateCharacterAsync(1, "c2"); // Existing

            // Assert
            Assert.Equal(CharacterResult.CharacterCreatedSuccessfully(), result);
        }
        #endregion

        #region SetStory & SetDescription Tests
        [Fact]
        public async Task SetDescription_ValidInput_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterDescriptionAsync(1, "description"); // Existing
            var character = await controller.ShowCharacterAsync(1);

            // Assert
            Assert.True(CharacterResult.CharacterUpdatedSuccessfully().Equals(result) &&
                ((ShowCharacterViewModel)character.ViewModel).Description.Equals("description"));
        }

        [Fact]
        public async Task SetDescription_InvalidCharacter_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterDescriptionAsync(1000, "description");

            // Assert
            Assert.True(CharacterResult.CharacterNotFound().Equals(result));
        }

        [Fact]
        public async Task SetDescription_NullInput_ReturnInvalidInput()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterDescriptionAsync(1, null); // Existing
            var character = await controller.ShowCharacterAsync(1);

            // Assert
            Assert.True(GenericResult.InvalidInput().Equals(result));
        }

        [Fact]
        public async Task SetStory_ValidInput_ReturnSuccess()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterStoryAsync(1, "story"); // Existing
            var character = await controller.ShowCharacterAsync(1);

            // Assert
            Assert.True(CharacterResult.CharacterUpdatedSuccessfully().Equals(result) &&
                ((ShowCharacterViewModel)character.ViewModel).Story.Equals("story"));
        }

        [Fact]
        public async Task SetStory_InvalidCharacter_ReturnCharacterNotFound()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterStoryAsync(1000, "description");

            // Assert
            Assert.True(CharacterResult.CharacterNotFound().Equals(result));
        }

        [Fact]
        public async Task SetStory_NullInput_ReturnInvalidInput()
        {
            // Arrange
            var provider = new MockCharacterProvider();
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            // Act
            var result = await controller.SetCharacterStoryAsync(1, null); // Existing
            var character = await controller.ShowCharacterAsync(1);

            // Assert
            Assert.True(GenericResult.InvalidInput().Equals(result));
        }
        #endregion

        #region GiveExperienceAsync Tests
        [Fact]
        public async Task GiveExperienceAsync_ValidInput_ExperienceEqualsFive()
        {
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                ExpEnabledChannels = new List<Channel> { new Channel(1) },
                ExpMessageLengthDivisor = 1
            };

            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var strategy = new GenericProgressionStrategy(statProvider, statOptions, campProvider);
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, new MockProgressionStrategy(), new GeneralOptions(), new MockUserProvider(), null);

            await controller.GiveExperienceAsync(1, 1, "12345");
            var character = await provider.GetActiveCharacterAsync(1);

            Assert.True(character.Experience == 5);
        }

        [Fact]
        public async Task GiveExperienceAsync_MessageIsWhitespace_ExperienceEqualsZero()
        {
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                ExpEnabledChannels = new List<Channel> { new Channel(1) },
                ExpMessageLengthDivisor = 1
            };
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var strategy = new GenericProgressionStrategy(statProvider, statOptions, campProvider);
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, strategy, new GeneralOptions(), new MockUserProvider() , null);

            await controller.GiveExperienceAsync(1, 1, "               ");
            var character = await provider.GetActiveCharacterAsync(1);

            Assert.True(character.Experience == 0);
        }

        [Fact]
        public async Task GiveExperienceAsync_NotInEnabledChannel_ExperienceEqualsZero()
        {
            var provider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                ExpEnabledChannels = new List<Channel> { new Channel(1) },
                ExpMessageLengthDivisor = 1
            };
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var strategy = new GenericProgressionStrategy(statProvider, statOptions, campProvider);
            //var campaignProvider = new MockCampaignProvider();
            var controller = new CharacterController(provider, strategy, new GeneralOptions(), new MockUserProvider() , null);

            await controller.GiveExperienceAsync(1, 2, "12345");
            var character = await provider.GetActiveCharacterAsync(1);

            Assert.True(character.Experience == 0);
        }
        #endregion
    }
}
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
    public class EffectControllerTests
    {
        private readonly ITestOutputHelper output;

        public EffectControllerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region AddEffect Tests
        [Fact]
        public async Task AddEffectAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            var effect = await effectProvider.CreateEffectAsync(1, "ValidInput", null);

            // Act
            await controller.AddEffectAsync(1, "ValidInput");

            // Assert
            var character = await charProvider.GetActiveCharacterAsync(1);
            Assert.True(character.Effects.Contains(effect));
        }

        [Fact]
        public async Task AddEffectAsync_MultipleCharacters_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            
            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            var effect = await effectProvider.CreateEffectAsync(1, "ValidInput", null);

            // Act
            await controller.AddEffectAsync(1, "ValidInput");
            await controller.AddEffectAsync(2, "ValidInput");

            // Assert
            var character1 = await charProvider.GetActiveCharacterAsync(1);
            var character2 = await charProvider.GetActiveCharacterAsync(2);

            Assert.True(character1.Effects.Contains(effect) && character2.Effects.Contains(effect));
        }

        [Fact]
        public async Task AddEffectAsync_MultipleEffects_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            var effect = await effectProvider.CreateEffectAsync(1, "ValidInput", null);
            var effect2 = await effectProvider.CreateEffectAsync(1, "ValidInput2", null);

            // Act
            await controller.AddEffectAsync(1, "ValidInput");
            await controller.AddEffectAsync(1, "ValidInput2");

            // Assert
            var character = await charProvider.GetActiveCharacterAsync(1);

            Assert.True(character.Effects.Contains(effect) && character.Effects.Contains(effect2));
        }
        #endregion

        #region CreateEffect Tests
        [Fact]
        public async Task CreateEffectAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            var result = await controller.CreateEffectAsync(1, "ValidInput");

            Assert.Equal(EffectResult.EffectCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateEffectAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            await controller.CreateEffectAsync(1, "AlreadyExists");
            var result = await controller.CreateEffectAsync(1, "AlreadyExists");

            Assert.Equal(EffectResult.NameAlreadyExists(), result);
        }
        #endregion

        #region DeleteEffect Tests

        [Fact]
        public async Task DeleteEffect_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            var effectProvider = new MockEffectProvider();
            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            // Act
            await effectProvider.CreateEffectAsync(1, "ValidInput", null);
            var result = await controller.DeleteEffectAsync(1, "ValidInput");

            // Assert
            Assert.Equal(result, EffectResult.EffectDeletedSuccessfully());
        }

        [Fact]
        public async Task DeleteEffect_InvalidEffectName_ReturnNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            var effectProvider = new MockEffectProvider();
            var controller = new EffectController(charProvider, effectProvider, statProvider, new GeneralOptions(), campProvider);

            // Act
            var result = await controller.DeleteEffectAsync(1, "DoesNotExist");

            // Assert
            Assert.Equal(result, EffectResult.EffectNotFound());
        }
        #endregion

        #region GetEffect Tests
        #endregion

        #region SetStatisticEffect Tests
        [Fact]
        public async Task SetStatisticEffectAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new EffectController(charProvider, effectProvider, statProvider, null, campProvider);

            await effectProvider.CreateEffectAsync(1, "ValidInput", null);
            var result = await controller.SetStatisticEffectAsync(1, "ValidInput", "Strength", 1);

            Assert.Equal(EffectResult.EffectUpdatedSucessfully(), result);
        }

        [Fact]
        public async Task SetStatisticEffectAsync_InvalidEffectName_ReturnEffectNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new EffectController(charProvider, effectProvider, statProvider, null, campProvider);

            var result = await controller.SetStatisticEffectAsync(1, "DoesNotExist", "Strength", 1);

            Assert.Equal(EffectResult.EffectNotFound(), result);
        }

        [Fact]
        public async Task SetStatisticEffectAsync_InvalidStatisticName_ReturnStatisticNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var effectProvider = new MockEffectProvider();
            var statProvider = new MockStatisticProvider();
            var userProvider = new MockUserProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new EffectController(charProvider, effectProvider, statProvider, null, campProvider);

            await effectProvider.CreateEffectAsync(1, "ValidInput", null);
            var result = await controller.SetStatisticEffectAsync(1, "ValidInput", "DoesNotExist", 1);

            Assert.Equal(StatisticResult.StatisticNotFound(), result);
        }
        #endregion
    }
}
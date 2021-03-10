using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
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
    public class StatisticControllerTests
    {
        private readonly ITestOutputHelper output;

        public StatisticControllerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region CreateAttribute & CreateStatistic Tests
        [Fact]
        public async Task CreateAttributeAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateAttributeAsync("Wisdom");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateAttributeAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateAttributeAsync("Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Intimidation", "Strength");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Powerlifting", "Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_InvalidAttributeName_ReturnCreationFailed()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Intimidation", "STR");

            Assert.Equal(StatisticResult.StatisticCreationFailed(), result);
        }
        #endregion

        

        #region DeleteStatistic Tests
        
        [Fact]
        public async Task DeleteStatistic_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            // Act
            var result = await controller.DeleteStatisticAsync("strength");

            // Assert
            Assert.Equal(result, StatisticResult.StatisticDeletedSuccessfully());
        }

        [Fact]
        public async Task DeleteStatistic_InvalidStatName_ReturnNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            // Act
            var result = await controller.DeleteStatisticAsync("bacon");

            // Assert
            Assert.Equal(result, StatisticResult.StatisticNotFound());
        }
        #endregion
    
        #region Campaign Tests

        [Fact]
        public async Task CreateCampaignAttributeAsync_ValidInput_ReturnSuccess()
        {
            var userProvider = new MockUserProvider();
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            await campProvider.CreateCampaignAsync(1, "thecamp", 1);

            var controller = new StatisticController(charProvider, statProvider, campProvider);

            var result = await controller.CreateCampaignAttributeAsync("Wisdom", 1, 1);

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateCampaignAttributeAsync_CampaignDoesNotExist_ReturnNotFoundByChannel()
        {
            var userProvider = new MockUserProvider();
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var campProvider = new MockCampaignProvider(userProvider);

            var controller = new StatisticController(charProvider, statProvider, campProvider);

            var result = await controller.CreateCampaignAttributeAsync("Wisdom", 1, 1);

            Assert.Equal(CampaignResult.NotFoundByChannel(), result);
        }

        [Fact]
        public async Task RenameStatisticAsync_InCampaign_ReturnNotFound()
        {
            // Arrange
            var userProvider = new MockUserProvider();
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var campProvider = new MockCampaignProvider(userProvider);
            var controller = new StatisticController(charProvider, statProvider, campProvider);

            string attributeName = "Wisdom", newAttribName = "Intellect";

            string campaignName = "thecamp";

            // Act
            await campProvider.CreateCampaignAsync(0, campaignName, 0);
            Campaign campaign = await campProvider.GetCampaignAsync(campaignName);

            await statProvider.CreateAttributeAsync(attributeName, campaign);
            
            var result = await controller.RenameStatisticAsync(attributeName, newAttribName);

            // Assert
            Assert.Equal(StatisticResult.StatisticNotFound(), result);
        }

        #endregion
    }
}
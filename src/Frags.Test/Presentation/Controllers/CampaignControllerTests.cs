using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels.Campaigns;
using Xunit;

namespace Frags.Test.Presentation.Controllers
{
    public class CampaignControllerTests
    {
        [Fact]
        public async Task AddCampaignChannel_ValidInput_ReturnSuccess()
        {
            var userProv = new MockUserProvider();
            var charProv = new MockCharacterProvider();
            var provider = new MockCampaignProvider(userProv);
            var controller = new CampaignController(userProv, charProv, provider);

            string name = nameof(CampaignControllerTests.AddCampaignChannel_ValidInput_ReturnSuccess);
            ulong userId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            ulong channelId = (ulong)GameRandom.Between(11, int.MaxValue - 1);

            await controller.CreateCampaignAsync(userId, name);
            var result = await controller.AddCampaignChannelAsync(name, channelId);

            Assert.Equal(CampaignResult.ChannelAdded(), result);
        }

        [Fact]
        public async Task CreateCampaignAsync_ValidInput_ReturnSuccess()
        {
            ulong userId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            ulong channelId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            string name = nameof(CampaignControllerTests.CreateCampaignAsync_ValidInput_ReturnSuccess);

            var userProv = new MockUserProvider();
            var charProv = new MockCharacterProvider();

            var provider = new MockCampaignProvider(userProv);
            var strategy = new MockProgressionStrategy();
            var controller = new CampaignController(userProv, charProv, provider);

            var result = await controller.CreateCampaignAsync(userId, name);

            Assert.Equal(CampaignResult.CampaignCreated(), result);
        }

        [Fact]
        public async Task CreateCampaignAsync_DuplicateName_ReturnFailure()
        {
            ulong userId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            ulong channelId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            string name = nameof(CampaignControllerTests.CreateCampaignAsync_DuplicateName_ReturnFailure);

            var userProv = new MockUserProvider();
            var charProv = new MockCharacterProvider();

            var provider = new MockCampaignProvider(userProv);
            var strategy = new MockProgressionStrategy();
            var controller = new CampaignController(userProv, charProv, provider);

            await controller.CreateCampaignAsync(userId, name);
            var result = await controller.CreateCampaignAsync(userId, name);

            Assert.Equal(CampaignResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task GetCampaignInfo_ValidInput_ReturnSuccess()
        {
            ulong userId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            ulong channelId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            string name = nameof(CampaignControllerTests.GetCampaignInfo_ValidInput_ReturnSuccess);
            //string charName = "Bob";

            var userProv = new MockUserProvider();
            var charProv = new MockCharacterProvider();
            await charProv.CreateCharacterAsync(userId, name);

            var provider = new MockCampaignProvider(userProv);
            var strategy = new MockProgressionStrategy();
            var stratName = strategy.GetType().Name;
            var controller = new CampaignController(userProv, charProv, provider);

            await controller.CreateCampaignAsync(userId, name);
            await controller.AddCampaignChannelAsync(name, channelId);
            await controller.ConfigureCampaignAsync(userId, channelId, nameof(StatisticOptions.ProgressionStrategy), stratName);
            var convResult = await controller.ConvertCharacterAsync(userId, channelId);
            var result = await controller.GetCampaignInfoAsync(name);
            var viewModel = (ShowCampaignViewModel)result.ViewModel;
            
            bool nameEq = viewModel.Name.Equals(name);
            bool chanExist = viewModel.Channels.Exists(x => x.Id == channelId);
            bool charNameFound = viewModel.CharacterNames.Any(x => x.Equals(name));
            bool userIdEq = viewModel.Owner.UserIdentifier == userId;
            bool progEq = viewModel.StatisticOptions.ProgressionStrategy.Equals(stratName);

            Assert.True(nameEq && chanExist && charNameFound && userIdEq && progEq);
        }
    }
}
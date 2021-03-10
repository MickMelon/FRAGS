
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels.Campaigns;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Test.Database.DataAccess
{
    public class EfCampaignProviderTests
    {
        private readonly ITestOutputHelper _output;

        EfUserProvider userProvider = null;
        EfStatisticProvider statProvider = null;
        EfEffectProvider effectProvider = null;
        EfCharacterProvider charProvider = null;
        EfCampaignProvider campProvider = null;

        List<IProgressionStrategy> progStrats = new List<IProgressionStrategy>{ new MockProgressionStrategy() };
        List<IRollStrategy> rollStrats = new List<IRollStrategy> { new MockRollStrategy() };

        public EfCampaignProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task RenameCampaignAsync_ValidInput_ReturnSuccess()
        {
            ulong userId = 69;
            ulong channelId = 420;

            _output.WriteLine("User id: " + userId);
            _output.WriteLine("Channel id: " + channelId);

            string baseName = nameof(EfCampaignProviderTests.RenameCampaignAsync_ValidInput_ReturnSuccess);
            string campName = baseName += "_Campaign";

            var options = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = baseName + "_DB"
            };

            

            // $camp create
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                await campProvider.CreateCampaignAsync(userId, campName);
            }

            // $camp addchannel
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, null);

                var result = await campController.AddCampaignChannelAsync(campName, channelId);
                if (!result.IsSuccess) throw new CampaignException(result.Message);
            }

            // $camp rename
            string newName = "the new name";
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, null);
                
                var result = await campController.RenameCampaignAsync(userId, channelId, newName);
                if (!result.IsSuccess) throw new CampaignException(result.Message);
            }

            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, null);

                Campaign campaign = await campProvider.GetCampaignAsync(channelId);

                Assert.Equal(newName, campaign.Name);
            }
        }

        [Fact]
        public async Task TheBigOne()
        {
            // Each using statement simulates a scoped DI request per command

            string baseName = nameof(EfCampaignProviderTests.TheBigOne);

            var options = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = baseName + "_DB"
            };

            ulong userId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            ulong channelId = (ulong)GameRandom.Between(11, int.MaxValue - 1);
            string campName = baseName + "_Campaign";

            var rollStrats = new List<IRollStrategy> { new MockRollStrategy() };
            var progStrats = new List<IProgressionStrategy>{ new MockProgressionStrategy() };

            _output.WriteLine($"User ID: {userId}, Channel ID: {channelId}");

            // $camp create
            using (var context = new RpgContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                RefreshState(context);

                await campProvider.CreateCampaignAsync(userId, campName);
            }

            // $camp addchannel
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, null);

                var result = await campController.AddCampaignChannelAsync(campName, channelId);
                if (!result.IsSuccess) throw new CampaignException(result.Message);
            }

            // $camp statoptions
            using (var context = new RpgContext(options))
            {
                RefreshState(context);
                
                var campController = new CampaignController(userProvider, charProvider, campProvider, null);

                var propertyName = nameof(StatisticOptions.ProgressionStrategy);
                string newValue = nameof(MockProgressionStrategy);

                var result = await campController.ConfigureStatisticOptionsAsync(userId, channelId, propertyName, newValue);
                if (!result.IsSuccess) throw new CampaignException(result.Message);
            }

            string charName = baseName + "_Character";

            // $create (character)
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                await charProvider.CreateCharacterAsync(userId, charName);
            }

            // $camp convert
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, null);

                var result = await campController.ConvertCharacterAsync(userId, channelId);
                if (!result.IsSuccess) throw new CampaignException(result.Message);
            }

            // $camp info
            using (var context = new RpgContext(options))
            {
                RefreshState(context);

                var campController = new CampaignController(userProvider, charProvider, campProvider, statProvider);
                var result = await campController.GetCampaignInfoAsync(campName);
                ShowCampaignViewModel vm = (ShowCampaignViewModel)result.ViewModel;
                
                bool nameEq = vm.Name.Equals(campName);
                bool chanExist = vm.Channels.Exists(x => x.Id == channelId);
                bool charNameFound = vm.CharacterNames.Any(x => x.Equals(charName));
                bool userIdEq = vm.Owner.UserIdentifier == userId;
                bool progEq = vm.StatisticOptions.ProgressionStrategy.Equals(nameof(MockProgressionStrategy));

                Assert.True(nameEq && chanExist && charNameFound && userIdEq && progEq);
            }
        }

        private void RefreshState(RpgContext context)
        {
            userProvider = new EfUserProvider(context);
            statProvider = new EfStatisticProvider(context);
            effectProvider = new EfEffectProvider(context, userProvider, statProvider);
            charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);
            campProvider = new EfCampaignProvider(context, progStrats, rollStrats);
        }
    }
}

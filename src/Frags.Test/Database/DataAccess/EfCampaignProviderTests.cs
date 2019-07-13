using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using System;
using System.Collections.Generic;
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

        public EfCampaignProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task EntityFramework_CreateCampaign_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CreateCampaign_EntityMatchesInput"
            });

            var provider = new EfCampaignProvider(context);

            var campaign = await provider.CreateCampaignAsync(1, "bottom text");
            campaign.Channels = new List<ulong> { 21, 22, 23, 24 };
            campaign.Characters = new List<Character> { new Character(1, "bob"), new Character(2, "jane"), new Character(3, "cal") };
            campaign.Effects = new List<Effect> { new Effect(1, "bob's effect"), new Effect(2, "jane's effect"), new Effect(3, "cal's effect") };
            campaign.ModeratorUserIdentifiers = new List<ulong> { 31, 32, 33 };
            campaign.Name = "coolcampaign";
            campaign.OwnerUserIdentifier = 42;
            campaign.RollOptions = new RollOptions { RollStrategy = "frags" };
            campaign.Statistics = new List<Statistic> { new Attribute("Strength"), new Attribute("Intelligence") };

            var expChannels = new ulong[] { 2, 3, 4, 5 };
            campaign.StatisticOptions = new StatisticOptions
            {
                AttributeMax = 1,
                ExpEnabledChannels = expChannels,
                ProgressionStrategy = "vegas",
            };
            await provider.UpdateCampaignAsync(campaign);

            campaign = await provider.GetCampaignAsync(1);
            var campChannels = await provider.GetChannelsAsync(1);
            var characters = await provider.GetCharactersAsync(1);
            var effects = await provider.GetEffectsAsync(1);
            var moderators = await provider.GetModeratorsAsync(1);
            var rollOptions = await provider.GetRollOptionsAsync(1);
            var statistics = await provider.GetStatisticsAsync(1);
            var statOptions = await provider.GetStatisticOptionsAsync(1);

            Assert.True(campaign.Name == "coolcampaign" &&
                campChannels.Count == 8 &&
                characters.Count == 3 &&
                effects.Count == 3 &&
                moderators.Count == 3 &&
                rollOptions.RollStrategy == "frags" &&
                statistics.Count == 2 &&
                statOptions.ExpEnabledChannels.Length == 4);
        }
    }
}

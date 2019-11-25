using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.AutoMapper;
using Frags.Database.DataAccess;
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

        public EfCampaignProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task EntityFramework_CreateCampaign_EntityMatchesInput()
        {
            var mapperConfig = new MapperConfiguration(x => x.AddProfile<GeneralProfile>());
            var mapper = new Mapper(mapperConfig);
            var genOptions = new GeneralOptions { UseInMemoryDatabase = true, DatabaseName = "CreateCampaign_EntityMatchesInput" };

            using (var context = new RpgContext(genOptions))
            {
                context.Database.EnsureCreated();
                var provider = new EfCampaignProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper);

                await charProvider.CreateCharacterAsync(1, "bob");
                await charProvider.CreateCharacterAsync(2, "jane");
                await charProvider.CreateCharacterAsync(3, "cal");
            }

            using (var context = new RpgContext(genOptions))
            {
                var provider = new EfCampaignProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper);

                await provider.CreateCampaignAsync(1, "bottom text");
            }

            Campaign campaign;
            using (var context = new RpgContext(genOptions))
            {
                var provider = new EfCampaignProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper);
                campaign = await provider.GetCampaignAsync(1);

                User bob = await charProvider.JustGetTheDamnUser(1);
                User jane = await charProvider.JustGetTheDamnUser(2);
                User cal = await charProvider.JustGetTheDamnUser(3);

                campaign.Channels = new List<Channel> { new Channel(21), new Channel(22), new Channel(23), new Channel(24) };
                campaign.Characters = new List<Character> { new Character(4, bob, false, "bob jr."), new Character(5, jane, false, "jane jr."), new Character(6, cal, false, "cal jr.") };
                campaign.Effects = new List<Effect> { new Effect(bob, "bob's effect") { Id = 10 }, new Effect(jane, "jane's effect") { Id = 20 }, new Effect(cal, "cal's effect") { Id = 30 }};
                campaign.ModeratedCampaigns = new List<Moderator> { new Moderator { Campaign = campaign, User = jane }, new Moderator { Campaign = campaign, User = cal } };
                campaign.Name = "coolcampaign";
                campaign.Owner = bob;
                campaign.RollOptions = new RollOptions { RollStrategy = "frags", Id = 11 };
                campaign.Statistics = new List<Statistic> { new Attribute("Strength") { Id = 12 }, new Attribute("Intelligence") { Id = 22 } };

                
                var expChannels = new ulong[] { 27, 37, 47, 57 };
                campaign.StatisticOptions = new StatisticOptions
                {
                    Id = 14,
                    AttributeMax = 1,
                    ExpEnabledChannels = expChannels,
                    ProgressionStrategy = "vegas",
                };
                

                await provider.UpdateCampaignAsync(campaign);
            }

            using (var context = new RpgContext(new GeneralOptions { UseInMemoryDatabase = true, DatabaseName = "CreateCampaign_EntityMatchesInput" }))
            {
                var provider = new EfCampaignProvider(context, mapper);
                var charProvider = new EfCharacterProvider(context, mapper);
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
                    moderators.Count() == 2 &&
                    rollOptions.RollStrategy == "frags" &&
                    statistics.Count == 2 &&
                    statOptions.ExpEnabledChannels.Length == 4);
            }
        }
    }
}

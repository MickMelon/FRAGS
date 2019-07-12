using Frags.Database;
using Frags.Database.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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

            Assert.Equal("bottom text", campaign.Name);
        }
    }
}

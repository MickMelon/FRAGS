using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Frags.Database.Characters;
using Frags.Database.DataAccess;
using Frags.Database.Repositories;
using Raven.TestDriver;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Database.DataAccess
{
    public class RavenDbTests : RavenTestDriver
    {
        private readonly ITestOutputHelper output;

        public RavenDbTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task RavenDb_CreateCharacter_EntityMatchesInput()
        {
            using (var store = GetDocumentStore())
            {
                var provider = new RavenDbCharacterProvider(store);

                await provider.CreateCharacterAsync(1, 305847674974896128, true, "Melon Head");

                WaitForIndexing(store);
                var result = await provider.GetActiveCharacterAsync(305847674974896128);

                Assert.True(result.UserIdentifier == 305847674974896128);
            }
        }
    }
}
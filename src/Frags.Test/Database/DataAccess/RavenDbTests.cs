using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Statistics;
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

                await provider.CreateCharacterAsync("1", 305847674974896128, true, "Melon Head");

                WaitForIndexing(store);
                var result = await provider.GetActiveCharacterAsync(305847674974896128);

                Assert.True(result.UserIdentifier == 305847674974896128);
            }
        }

        [Fact]
        public async Task RavenDb_CharacterStatistics_EntityMatchesInput()
        {
            using (var store = GetDocumentStore())
            {
                var provider = new RavenDbCharacterProvider(store);
                var statProvider = new RavenDbStatisticProvider(store);

                var strength = await statProvider.CreateAttributeAsync("Strength");
                var character = await provider.CreateCharacterAsync("1", 305847674974896128, true, "Melon Head");
                character.Statistics.Add(strength, new StatisticValue(5));
                WaitForIndexing(store);

                await provider.UpdateCharacterAsync(character);
                WaitForIndexing(store);
                var result = await provider.GetActiveCharacterAsync(305847674974896128);

                Assert.True(result.Statistics.Count > 0);
            }
        }

        [Fact]
        public async Task RavenDb_UpdateCharacter_EntityMatchesInput()
        {
            string id = "1",
            oldName = "Red",
            newName = "Mr. Red";

            ulong userIdentifier = 129306645548367872;

            using (var store = GetDocumentStore())
            {
                var provider = new RavenDbCharacterProvider(store);

                await provider.CreateCharacterAsync(id, userIdentifier, true, oldName);
                WaitForIndexing(store);

                Character result = await provider.GetActiveCharacterAsync(userIdentifier);
                result.Name = newName;
                await provider.UpdateCharacterAsync(result);
                WaitForIndexing(store);

                result = await provider.GetActiveCharacterAsync(userIdentifier);
                WaitForIndexing(store);

                Assert.Equal(newName, result.Name);
            }
        }
    }
}
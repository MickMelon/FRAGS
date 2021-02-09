using System.Collections.Generic;
using System.Threading.Tasks;

using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Database.DataAccess
{
    public class EfCharacterProviderTests
    {
        private readonly ITestOutputHelper _output;

        public EfCharacterProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Character Creation Tests
        [Fact]
        public async Task EntityFramework_CreateCharacter_EntityMatchesInput()
        {
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CreateCharacter_EntityMatchesInput"
            });

            var userProvider = new EfUserProvider(context);
            var statProvider = new EfStatisticProvider(context);
            var provider = new EfCharacterProvider(context, userProvider, statProvider);

            await provider.CreateCharacterAsync(305847674974896128, "Melon Head");
            var result = await provider.GetActiveCharacterAsync(305847674974896128);

            Assert.True(result.User.UserIdentifier == 305847674974896128);
        }

        [Fact]
        public async Task EntityFramework_CreateTwoCharacters_EntityMatchesInput()
        {
            int dbId = GameRandom.Between(5, int.MaxValue - 1);

            var genOpts = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CreateTwoCharacters_EntityMatchesInput" + dbId,
                CharacterLimit = 10
            };

            ulong userId = 305847674974896128;

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                var controller = new CharacterController(provider, null, genOpts, userProvider, null);
                await controller.CreateCharacterAsync(userId, "Thing 1");
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                var controller = new CharacterController(provider, null, genOpts, userProvider, null);
                await controller.CreateCharacterAsync(userId, "Thing 2");
            }

            IResult result;
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                var controller = new CharacterController(provider, null, genOpts, userProvider, null);
                result = await controller.ActivateCharacterAsync(userId, "Thing 1");
            }

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EntityFramework_CharacterStatistics_EntityMatchesInput()
        {
            var genOpts = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "CharacterStatistics_EntityMatchesInput"
            };

            ulong userIdentifier = 305847674974896128;
            string name = "Melon Head";

            //int id = 1;

            Attribute strength;
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                strength = await statProvider.CreateAttributeAsync("Strength");
            }

            var value = new StatisticValue(5);

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                await provider.CreateCharacterAsync(userIdentifier, name);
            }

            Character character;
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                character = await provider.GetActiveCharacterAsync(userIdentifier);
                character.SetStatistic(strength, value);
                await provider.UpdateCharacterAsync(character);
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                character = await provider.GetActiveCharacterAsync(userIdentifier);
                Assert.True(character.Statistics.Count > 0);
            }
        }

        [Fact]
        public async Task EntityFramework_UpdateCharacter_EntityMatchesInput()
        {
            var genOpts = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "UpdateCharacter_EntityMatchesInput"
            };

            ulong userIdentifier = 305847674974896128;
            string oldName = "Melon Head", newName = "Mr. Melon";
            //int id = 1;

            using (var context = new RpgContext(genOpts))
            {

                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);
                await provider.CreateCharacterAsync(userIdentifier, oldName);
            }

            using (var context = new RpgContext(genOpts))
            {

                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);

                var result = await provider.GetActiveCharacterAsync(userIdentifier);
                result.Name = newName;

                await provider.UpdateCharacterAsync(result);
            }

            using (var context = new RpgContext(genOpts))
            {

                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var provider = new EfCharacterProvider(context, userProvider, statProvider);

                var result = await provider.GetActiveCharacterAsync(userIdentifier);
                Assert.Equal(newName, result.Name);
            }
        }
        #endregion
    }
}
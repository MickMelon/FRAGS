using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using Frags.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Database.DataAccess
{
    public class EfEffectProviderTests
    {
        [Fact]
        public async Task MakeSureCharacterHasTheEffects()
        {
            var genOpts = new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "MakeSureCharacterHasTheEffects"
            };

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider);
                await charProvider.CreateCharacterAsync(1, "Char1");
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider);    
                var effect1 = await effectProvider.CreateEffectAsync(1, "Effect1");
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider);    
                var effect2 = await effectProvider.CreateEffectAsync(1, "Effect2");
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider);    

                var effect1 = await effectProvider.GetEffectAsync("Effect1");
                var effect2 = await effectProvider.GetEffectAsync("Effect2");

                var char1 = await charProvider.GetActiveCharacterAsync(1);

                char1.Effects.Add(effect1);
                char1.Effects.Add(effect2);

                await charProvider.UpdateCharacterAsync(char1);
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider);    

                var char1 = await charProvider.GetActiveCharacterAsync(1);
                Assert.True(char1.Effects.Count == 2);
            }
        }
    }
}
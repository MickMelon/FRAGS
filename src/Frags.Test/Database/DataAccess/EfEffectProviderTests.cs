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
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);
                await charProvider.CreateCharacterAsync(1, "Char1");
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);
                await charProvider.CreateCharacterAsync(2, "Char2");
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);    
                var effect1 = await effectProvider.CreateEffectAsync(1, "Effect1", null);
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);    
                var effect2 = await effectProvider.CreateEffectAsync(1, "Effect2", null);
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);    

                var effect1 = await effectProvider.GetEffectAsync("Effect1", null);
                var effect2 = await effectProvider.GetEffectAsync("Effect2", null);

                var char1 = await charProvider.GetActiveCharacterAsync(1);
                var char2 = await charProvider.GetActiveCharacterAsync(2);

                char1.Effects.Add(effect1);
                char1.Effects.Add(effect2);
                
                char2.Effects.Add(effect1);
                char2.Effects.Add(effect2);

                await charProvider.UpdateCharacterAsync(char1);
                await charProvider.UpdateCharacterAsync(char2);
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context);
                var statProvider = new EfStatisticProvider(context);
                var effectProvider = new EfEffectProvider(context, userProvider, statProvider);
                var charProvider = new EfCharacterProvider(context, userProvider, statProvider, effectProvider);    

                var char1 = await charProvider.GetActiveCharacterAsync(1);
                var char2 = await charProvider.GetActiveCharacterAsync(2);
                Assert.True(char1.Effects.Count == 2 && char2.Effects.Count == 2);
            }
        }
    }
}
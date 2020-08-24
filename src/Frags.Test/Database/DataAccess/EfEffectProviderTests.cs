using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.Characters;
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

            var mapperConfig = new MapperConfiguration(x => x.AddProfile<Frags.Database.AutoMapper.GeneralProfile>());
            var mapper = new Mapper(mapperConfig);

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context, mapper);
                var effectProvider = new EfEffectProvider(context, mapper, userProvider);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);    
                await charProvider.CreateCharacterAsync(1, "Char1");
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context, mapper);
                var effectProvider = new EfEffectProvider(context, mapper, userProvider);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);    
                var effect1 = await effectProvider.CreateEffectAsync(1, "Effect1");
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context, mapper);
                var effectProvider = new EfEffectProvider(context, mapper, userProvider);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);    
                var effect2 = await effectProvider.CreateEffectAsync(1, "Effect2");
            }
            
            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context, mapper);
                var effectProvider = new EfEffectProvider(context, mapper, userProvider);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);    

                var effect1 = await effectProvider.GetEffectAsync("Effect1");
                var effect2 = await effectProvider.GetEffectAsync("Effect2");

                var char1 = await charProvider.GetActiveCharacterAsync(1);

                char1.Effects.Add(effect1);
                char1.Effects.Add(effect2);

                await charProvider.UpdateCharacterAsync(char1);
            }

            using (var context = new RpgContext(genOpts))
            {
                var userProvider = new EfUserProvider(context, mapper);
                var effectProvider = new EfEffectProvider(context, mapper, userProvider);
                var charProvider = new EfCharacterProvider(context, mapper, userProvider);    

                var char1 = await charProvider.GetActiveCharacterAsync(1);
                Assert.True(char1.Effects.Count == 2);
            }
        }
    }
}
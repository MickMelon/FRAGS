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
            var context = new RpgContext(new GeneralOptions
            {
                UseInMemoryDatabase = true,
                DatabaseName = "MakeSureCharacterHasTheEffects"
            });

            var effectProvider = new EfEffectProvider(context);
            var charProvider = new EfCharacterProvider(context);

            var effect1 = await effectProvider.CreateEffectAsync(1, "Effect1");
            var effect2 = await effectProvider.CreateEffectAsync(1, "Effect2");
            var char1 = await charProvider.CreateCharacterAsync(1, "Char1");
            char1.Active = true;
            char1.Effects.Add(effect1);
            char1.Effects.Add(effect2);

            await charProvider.UpdateCharacterAsync(char1);

            char1 = await charProvider.GetActiveCharacterAsync(1);
            Assert.True(char1.Effects.Count == 2);
        }
    }
}
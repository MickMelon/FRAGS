using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Database;
using Frags.Database.Characters;
using Frags.Database.DataAccess;
using Frags.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Database.DataAccess
{
    public class EfCharacterProviderTests
    {
        private readonly ITestOutputHelper output;

        public EfCharacterProviderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Character Creation Tests
        [Fact]
        public async Task CanCreateCharacter()
        {
            var options = new DbContextOptionsBuilder<RpgContext>();
            options.UseInMemoryDatabase();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Character, CharacterDto>());
            var mapper = new Mapper(config);
            var context = new RpgContext(options.Options);
            var efRepo = new EfSqliteRepository<CharacterDto>(context);
            var provider = new EfCharacterProvider(mapper, efRepo);

            await provider.CreateCharacterAsync(2, "bob");
            var character = await provider.GetActiveCharacterAsync(2);
            
            output.WriteLine(character.Name);
            Assert.NotNull(character);
        }
        #endregion
    }
}
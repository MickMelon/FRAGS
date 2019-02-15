using Frags.Database.Characters;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database
{
    public class RpgContext : DbContext
    {
        public DbSet<CharacterDto> Characters { get; set; }

        public RpgContext(DbContextOptions<RpgContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Filename=test.db");
            }
        }
    }
}
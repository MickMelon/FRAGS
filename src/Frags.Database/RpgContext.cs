using System.Collections.Generic;
using Frags.Core.Characters;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database
{
    public class RpgContext : DbContext
    {
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StatisticMapping> StatisticMappings { get; set; }
        public DbSet<EffectMapping> EffectMappings { get; set; }

        private readonly GeneralOptions _options;

        public RpgContext(GeneralOptions options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options.UseInMemoryDatabase)
            {
                optionsBuilder.UseInMemoryDatabase(_options.DatabaseName);
            }
            else
            {
                optionsBuilder.UseSqlite($"Filename={_options.DatabaseName}.db");
            }

            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StatisticMapping>()
                .HasOne(x => x.Statistic)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Character>()
                .HasMany(c => c.Effects)
                .WithMany(e => e.Characters)
                .UsingEntity<EffectMapping>();
        }
    }
}
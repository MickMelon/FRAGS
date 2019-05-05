using System.Collections.Generic;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database
{
    public class RpgContext : DbContext
    {
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<CharacterDto> Characters { get; set; }
        public DbSet<EffectDto> Effects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> Users { get; set; }

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
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StatisticMapping>()
                .HasOne(x => x.Statistic)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EffectMapping>()
                .HasKey(ec => new { ec.EffectId, ec.CharacterId });

            builder.Entity<EffectMapping>()
                .HasOne(ec => ec.Effect)
                .WithMany(e => e.EffectMappings)
                .HasForeignKey(ec => ec.EffectId);

            builder.Entity<EffectMapping>()
                .HasOne(ec => ec.Character)
                .WithMany(c => c.EffectMappings)
                .HasForeignKey(ec => ec.CharacterId);

            builder.Entity<CharacterDto>().Metadata.FindNavigation(nameof(CharacterDto.EffectMappings)).IsEagerLoaded = true;
        }
    }
}
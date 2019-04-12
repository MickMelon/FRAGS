using System.Collections.Generic;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database
{
    public class RpgContext : DbContext
    {
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<CharacterDto> Characters { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> Users { get; set; }

        public RpgContext() {}

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
        }
    }
}
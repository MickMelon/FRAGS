using System.Collections.Generic;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database
{
    public class RpgContext : DbContext
    {
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StatisticList> StatisticLists { get; set; }
        public DbSet<EffectList> EffectLists { get; set; }
        public DbSet<Channel> Channels { get; set; }

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

            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // SQLite ignore case
            if (!_options.UseInMemoryDatabase)
                builder.UseCollation("NOCASE");

            builder.Entity<Character>()
                .Ignore(x => x.Statistics)
                .Ignore(x => x.Skills)
                .Ignore(x => x.Attributes)
                .Ignore(x => x.Effects);

            builder.Entity<Effect>()
                .Ignore(x => x.Statistics);

            builder.Entity<Moderator>()
                .HasKey(ec => new { ec.CampaignId, ec.UserId });

            builder.Entity<Moderator>()
                .HasOne(ec => ec.Campaign)
                .WithMany(e => e.ModeratedCampaigns)
                .HasForeignKey(ec => ec.CampaignId);

            builder.Entity<Moderator>()
                .HasOne(ec => ec.User)
                .WithMany(c => c.ModeratedCampaigns)
                .HasForeignKey(ec => ec.UserId);

            builder.Entity<Character>()
                .HasOne(ch => ch.Campaign)
                .WithMany(camp => camp.Characters)
                .IsRequired(false);

            builder.Entity<Character>()
                .HasOne(ch => ch.User)
                .WithMany(usr => usr.Characters)
                .IsRequired(true);
        }
    }
}
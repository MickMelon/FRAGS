﻿// <auto-generated />
using System;
using Frags.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Frags.Database.Migrations
{
    [DbContext(typeof(RpgContext))]
    partial class RpgContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("Frags.Core.Statistics.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Aliases");

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Statistics");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Statistic");
                });

            modelBuilder.Entity("Frags.Core.Statistics.StatisticMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CharacterDtoId");

                    b.Property<int?>("EffectDtoId");

                    b.Property<int?>("StatisticId");

                    b.Property<int?>("StatisticValueId");

                    b.HasKey("Id");

                    b.HasIndex("CharacterDtoId");

                    b.HasIndex("EffectDtoId");

                    b.HasIndex("StatisticId");

                    b.HasIndex("StatisticValueId");

                    b.ToTable("StatisticMapping");
                });

            modelBuilder.Entity("Frags.Core.Statistics.StatisticValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsProficient");

                    b.Property<double>("Proficiency");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.ToTable("StatisticValue");
                });

            modelBuilder.Entity("Frags.Database.Characters.CharacterDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("AttributePoints");

                    b.Property<string>("Description");

                    b.Property<int>("Experience");

                    b.Property<int>("Money");

                    b.Property<string>("Name");

                    b.Property<int>("SkillPoints");

                    b.Property<string>("Story");

                    b.Property<ulong>("UserIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Frags.Database.Characters.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActiveCharacterId");

                    b.Property<ulong>("UserIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("ActiveCharacterId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Frags.Database.Effects.EffectDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<ulong>("OwnerUserIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Effects");
                });

            modelBuilder.Entity("Frags.Database.Effects.EffectMapping", b =>
                {
                    b.Property<int>("EffectId");

                    b.Property<int>("CharacterId");

                    b.HasKey("EffectId", "CharacterId");

                    b.HasIndex("CharacterId");

                    b.ToTable("EffectMapping");
                });

            modelBuilder.Entity("Frags.Core.Statistics.Attribute", b =>
                {
                    b.HasBaseType("Frags.Core.Statistics.Statistic");

                    b.HasDiscriminator().HasValue("Attribute");
                });

            modelBuilder.Entity("Frags.Core.Statistics.Skill", b =>
                {
                    b.HasBaseType("Frags.Core.Statistics.Statistic");

                    b.Property<int?>("AttributeId");

                    b.Property<int>("MinimumValue");

                    b.HasIndex("AttributeId");

                    b.HasDiscriminator().HasValue("Skill");
                });

            modelBuilder.Entity("Frags.Core.Statistics.StatisticMapping", b =>
                {
                    b.HasOne("Frags.Database.Characters.CharacterDto")
                        .WithMany("Statistics")
                        .HasForeignKey("CharacterDtoId");

                    b.HasOne("Frags.Database.Effects.EffectDto")
                        .WithMany("StatisticEffects")
                        .HasForeignKey("EffectDtoId");

                    b.HasOne("Frags.Core.Statistics.Statistic", "Statistic")
                        .WithMany()
                        .HasForeignKey("StatisticId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Frags.Core.Statistics.StatisticValue", "StatisticValue")
                        .WithMany()
                        .HasForeignKey("StatisticValueId");
                });

            modelBuilder.Entity("Frags.Database.Characters.User", b =>
                {
                    b.HasOne("Frags.Database.Characters.CharacterDto", "ActiveCharacter")
                        .WithMany()
                        .HasForeignKey("ActiveCharacterId");
                });

            modelBuilder.Entity("Frags.Database.Effects.EffectMapping", b =>
                {
                    b.HasOne("Frags.Database.Characters.CharacterDto", "Character")
                        .WithMany("EffectMappings")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Frags.Database.Effects.EffectDto", "Effect")
                        .WithMany("EffectMappings")
                        .HasForeignKey("EffectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Frags.Core.Statistics.Skill", b =>
                {
                    b.HasOne("Frags.Core.Statistics.Attribute", "Attribute")
                        .WithMany()
                        .HasForeignKey("AttributeId");
                });
#pragma warning restore 612, 618
        }
    }
}

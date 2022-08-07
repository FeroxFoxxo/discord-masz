﻿// <auto-generated />
using Levels.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Levels.Migrations
{
    [DbContext(typeof(LevelsDatabase))]
    partial class LevelsDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Levels")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Levels.Models.GuildLevelConfig", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Coefficients")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DisabledXpChannels")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("HandleRoles")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LevelUpMessageOverrides")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LevelUpTemplate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Levels")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("MaximumTextXpGiven")
                        .HasColumnType("int");

                    b.Property<int>("MaximumVoiceXpGiven")
                        .HasColumnType("int");

                    b.Property<int>("MinimumTextXpGiven")
                        .HasColumnType("int");

                    b.Property<int>("MinimumVoiceXpGiven")
                        .HasColumnType("int");

                    b.Property<ulong>("NicknameDisabledReplacement")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("NicknameDisabledRole")
                        .HasColumnType("bigint unsigned");

                    b.Property<bool>("SendTextLevelUps")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("SendVoiceLevelUps")
                        .HasColumnType("tinyint(1)");

                    b.Property<ulong>("TextLevelUpChannel")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("VoiceLevelUpChannel")
                        .HasColumnType("bigint unsigned");

                    b.Property<bool>("VoiceXpCountMutedMembers")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("VoiceXpRequiredMembers")
                        .HasColumnType("int");

                    b.Property<int>("XpInterval")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("GuildLevelConfigs", "Levels");
                });

            modelBuilder.Entity("Levels.Models.GuildUserLevel", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("char(22)");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("TextXp")
                        .HasColumnType("bigint");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("VoiceXp")
                        .HasColumnType("bigint");

                    b.HasKey("Token");

                    b.ToTable("GuildUserLevels", "Levels");
                });

            modelBuilder.Entity("Levels.Models.UserRankcardConfig", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Background")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<uint>("LevelBgColor")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("OffColor")
                        .HasColumnType("int unsigned");

                    b.Property<int>("RankcardFlags")
                        .HasColumnType("int");

                    b.Property<uint>("TitleBgColor")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("XpColor")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.ToTable("UserRankcardConfigs", "Levels");
                });
#pragma warning restore 612, 618
        }
    }
}

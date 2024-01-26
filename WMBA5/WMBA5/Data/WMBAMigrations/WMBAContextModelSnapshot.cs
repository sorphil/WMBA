﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WMBA5.Data;

#nullable disable

namespace WMBA5.Data.WMBAMigrations
{
    [DbContext(typeof(WMBAContext))]
    partial class WMBAContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("WMBA5.Models.Club", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClubName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("WMBA5.Models.Coach", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoachName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Coaches");
                });

            modelBuilder.Entity("WMBA5.Models.Division", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClubID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DivisionName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ClubID");

                    b.ToTable("Divisions");
                });

            modelBuilder.Entity("WMBA5.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DivisionID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("Oponent")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Outcome")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayingAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RosterID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("DivisionID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("WMBA5.Models.Inning", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InningNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayersOut")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RunsScored")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.ToTable("Innings");
                });

            modelBuilder.Entity("WMBA5.Models.Player", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("JerseyNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("MemberID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<int>("RosterID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("MemberID")
                        .IsUnique();

                    b.HasIndex("TeamID");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("WMBA5.Models.PlayerAtBat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InningID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Result")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.HasIndex("InningID");

                    b.HasIndex("PlayerID");

                    b.ToTable("PlayerAtBats");
                });

            modelBuilder.Entity("WMBA5.Models.PlayerStat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Hits")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerApperance")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RBI")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RunsScored")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StrikeOuts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Walks")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PlayerID");

                    b.ToTable("PlayerStats");
                });

            modelBuilder.Entity("WMBA5.Models.Roster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BattingOrder")
                        .HasColumnType("TEXT");

                    b.Property<string>("FieldingPosition")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPlaying")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("GameID")
                        .IsUnique();

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.HasIndex("TeamID")
                        .IsUnique();

                    b.ToTable("Rosters");
                });

            modelBuilder.Entity("WMBA5.Models.Team", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CoachID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DivisionID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RosterID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CoachID");

                    b.HasIndex("DivisionID");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("WMBA5.Models.Division", b =>
                {
                    b.HasOne("WMBA5.Models.Club", "Club")
                        .WithMany("Divisions")
                        .HasForeignKey("ClubID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("WMBA5.Models.Game", b =>
                {
                    b.HasOne("WMBA5.Models.Division", "Division")
                        .WithMany("Games")
                        .HasForeignKey("DivisionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Division");
                });

            modelBuilder.Entity("WMBA5.Models.Inning", b =>
                {
                    b.HasOne("WMBA5.Models.Game", "Game")
                        .WithMany("Innings")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("WMBA5.Models.Player", b =>
                {
                    b.HasOne("WMBA5.Models.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("WMBA5.Models.PlayerAtBat", b =>
                {
                    b.HasOne("WMBA5.Models.Game", "Game")
                        .WithMany("PlayerAtBats")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WMBA5.Models.Inning", "Inning")
                        .WithMany("PlayerAtBats")
                        .HasForeignKey("InningID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WMBA5.Models.Player", "Player")
                        .WithMany("PlayerAtBats")
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Inning");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("WMBA5.Models.PlayerStat", b =>
                {
                    b.HasOne("WMBA5.Models.Player", "Player")
                        .WithMany("PlayerStats")
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("WMBA5.Models.Roster", b =>
                {
                    b.HasOne("WMBA5.Models.Game", "Game")
                        .WithOne("Roster")
                        .HasForeignKey("WMBA5.Models.Roster", "GameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMBA5.Models.Player", "Player")
                        .WithOne("Roster")
                        .HasForeignKey("WMBA5.Models.Roster", "PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMBA5.Models.Team", "Team")
                        .WithOne("Roster")
                        .HasForeignKey("WMBA5.Models.Roster", "TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Player");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("WMBA5.Models.Team", b =>
                {
                    b.HasOne("WMBA5.Models.Coach", "Coach")
                        .WithMany("Teams")
                        .HasForeignKey("CoachID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WMBA5.Models.Division", "Division")
                        .WithMany("Teams")
                        .HasForeignKey("DivisionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Coach");

                    b.Navigation("Division");
                });

            modelBuilder.Entity("WMBA5.Models.Club", b =>
                {
                    b.Navigation("Divisions");
                });

            modelBuilder.Entity("WMBA5.Models.Coach", b =>
                {
                    b.Navigation("Teams");
                });

            modelBuilder.Entity("WMBA5.Models.Division", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("WMBA5.Models.Game", b =>
                {
                    b.Navigation("Innings");

                    b.Navigation("PlayerAtBats");

                    b.Navigation("Roster");
                });

            modelBuilder.Entity("WMBA5.Models.Inning", b =>
                {
                    b.Navigation("PlayerAtBats");
                });

            modelBuilder.Entity("WMBA5.Models.Player", b =>
                {
                    b.Navigation("PlayerAtBats");

                    b.Navigation("PlayerStats");

                    b.Navigation("Roster");
                });

            modelBuilder.Entity("WMBA5.Models.Team", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Roster");
                });
#pragma warning restore 612, 618
        }
    }
}

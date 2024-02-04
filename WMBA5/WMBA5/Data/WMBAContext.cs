using Microsoft.EntityFrameworkCore;
using WMBA5.Models;

namespace WMBA5.Data
{
    public class WMBAContext : DbContext
    {
        public WMBAContext(DbContextOptions<WMBAContext> options) : base(options) 
        { 
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<PlayerStat> PlayerStats { get; set; }
        public DbSet<PlayerAtBat> PlayerAtBats { get; set; }
        public DbSet<Inning> Innings { get; set; }
        public DbSet<Lineup> Lineups { get; set; }

        public DbSet<Status> Statuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           //Prevent Cascade Delete
           //from Club to Division
           modelBuilder.Entity<Club>()
                .HasMany<Division>(c => c.Divisions)
                .WithOne(c => c.Club)
                .HasForeignKey(c => c.ClubID)
                .OnDelete(DeleteBehavior.Restrict);

            //Division to Teams
            modelBuilder.Entity<Division>()
                .HasMany<Team>(c => c.Teams)
                .WithOne(c => c.Division)
                .HasForeignKey(c => c.DivisionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Team to Player
            modelBuilder.Entity<Team>()
                .HasMany<Player>(c => c.Players)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamID)
                .OnDelete(DeleteBehavior.Restrict);

            //Coach To Team
            modelBuilder.Entity<Coach>()
                .HasMany<Team>(c => c.Teams)
                .WithOne(c => c.Coach)
                .HasForeignKey(c => c.CoachID)
                .OnDelete(DeleteBehavior.Restrict);

            //Division to Game
            modelBuilder.Entity<Division>()
                .HasMany<Game>(c => c.Games)
                .WithOne(c => c.Division)
                .HasForeignKey(c => c.DivisionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Player to PlayerStats
            modelBuilder.Entity<Player>()
                .HasMany<PlayerStat>(c => c.PlayerStats)
                .WithOne(c => c.Player)
                .HasForeignKey(c => c.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            //Player to PlayerAtBat
            modelBuilder.Entity<Player>()
                .HasMany<PlayerAtBat>(c => c.PlayerAtBats)
                .WithOne(c => c.Player)
                .HasForeignKey(c => c.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            //Game to Innings
            modelBuilder.Entity<Game>()
                .HasMany<Inning>(c => c.Innings)
                .WithOne(c => c.Game)
                .HasForeignKey(c => c.GameID)
                .OnDelete(DeleteBehavior.Restrict);

            //Game to PlayerAtBat
            modelBuilder.Entity<Game>()
                .HasMany<PlayerAtBat>(c => c.PlayerAtBats)
                .WithOne(c => c.Game)
                .HasForeignKey(c => c.GameID)
                .OnDelete(DeleteBehavior.Restrict);

            //Inning to PlayerAtBat
            modelBuilder.Entity<Inning>()
                .HasMany<PlayerAtBat>(c => c.PlayerAtBats)
                .WithOne(c => c.Inning)
                .HasForeignKey(c => c.InningID)
                .OnDelete(DeleteBehavior.Restrict);


            ////Game to Lineup
            //modelBuilder.Entity<Game>()
            //    .HasOne(c => c.Lineup)
            //    .WithOne( c => c.Game)
            //    .HasForeignKey<Lineup>(c => c.GameID)
            //    .IsRequired();

            ////Player to Lineup
            //modelBuilder.Entity<Player>()
            //    .HasOne(c => c.Lineup)
            //    .WithOne(c => c.Player)
            //    .HasForeignKey<Lineup>(c => c.PlayerID)
            //    .IsRequired();

            //Team to Lineup
            modelBuilder.Entity<Team>()
                .HasMany<Lineup>(c => c.Lineups)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamID)
                .IsRequired();


            //To Ensure Players has Unique MemberID
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.MemberID)
                .IsUnique();

			modelBuilder.Entity<TeamGame>()
				.HasOne(tg => tg.AwayTeam)
				.WithMany(t => t.AwayTeamGames)
				.HasForeignKey(tg => tg.AwayTeamID)
				.OnDelete(DeleteBehavior.Restrict);
		}
    }
}

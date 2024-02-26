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

        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<GameScore> GameScores { get; set; }
        public DbSet<Inning> Innings { get; set; }

        //public DbSet<Lineup> Lineups { get; set; }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
        public DbSet<Player> Players { get; set; }

        //public DbSet<PlayerAtBat> PlayerAtBats { get; set; }

        public DbSet<PlayerScore> PlayerScores { get; set; }
        //public DbSet<PlayerLineup> PlayerLineups { get; set; }

        public DbSet<PlayerStat> PlayerStats { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<Status> Statuses { get; set; }
        public DbSet<Team> Teams { get; set; }


        //public DbSet<TeamLineup> TeamLineups { get; set; } // this is a enum not a class Dbset is not required for this.



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

            ////Inning to PlayerAtBat
            //modelBuilder.Entity<Inning>()
            //    .HasMany<PlayerAtBat>(c => c.PlayerAtBats)
            //    .WithOne(c => c.Inning)
            //    .HasForeignKey(c => c.InningID)
            //    .OnDelete(DeleteBehavior.Restrict);

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
            //modelBuilder.Entity<Team>()
            //    .HasMany<Lineup>(c => c.Lineups)
            //    .WithOne(c => c.Team)
            //    .HasForeignKey(c => c.TeamID)
            //    .IsRequired();


            //To Ensure Players has Unique MemberID
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.MemberID)
                .IsUnique();

            //modelBuilder.Entity<TeamGame>()
            //    .HasOne(tg => tg.AwayTeam)
            //    .WithMany(t => t.AwayTeamGames)
            //    .HasForeignKey(tg => tg.AwayTeamID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the Game Player
            modelBuilder.Entity<GamePlayer>()
                .HasIndex(c => new { c.PlayerID, c.GameID })
                .IsUnique();

            //Game to innings
            modelBuilder.Entity<Game>()
                .HasMany<Inning>(g => g.Innings)
                .WithOne(g => g.Game)
                .HasForeignKey(g => g.GameID)
                .OnDelete(DeleteBehavior.Restrict);

            ////Game to InGameStats
            //modelBuilder.Entity<Game>()
            //    .HasMany<InGameStats> (g => g.InGameStats)
            //    .WithOne(g => g.Game)
            //    .HasForeignKey(g => g.GameID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Game to GameScore
            modelBuilder.Entity<Game>()
                .HasOne<GameScore>(g => g.GameScore)
                .WithOne(gs => gs.Game)
                .HasForeignKey<GameScore>(gs => gs.GameID);

            //Game to GamePlayer
            modelBuilder.Entity<Game>()
                .HasMany<GamePlayer>(g => g.GamePlayers)
                .WithOne(g => g.Game)
                .HasForeignKey(g => g.GameID)
                .OnDelete(DeleteBehavior.Restrict);

            //Player to GamePlayer
            modelBuilder.Entity<Player>()
                .HasMany<GamePlayer>(g => g.GamePlayers)
                .WithOne(g => g.Player)
                .HasForeignKey(g => g.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            //Player to PlayerGameScore
            modelBuilder.Entity<Player>()
                .HasMany<PlayerScore>(g => g.PlayerScores)
                .WithOne(g => g.Player)
                .HasForeignKey(g => g.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            ////GameScore to PlayerScore
            //modelBuilder.Entity<GameScore>()
            //    .HasMany<PlayerScore>(g => g.PlayerScores)
            //    .WithOne(g => g.GameScore)
            //    .HasForeignKey(g => g.GameScoreID)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

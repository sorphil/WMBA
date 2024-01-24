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


            //To Ensure Players has Unique MemberID
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.MemberID)
                .IsUnique();

        }
    }
}

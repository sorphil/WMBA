using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class GamePlayer
    {
        //Make sure you make the combination
        // of foreign keys integers unique.
        // You can do that in the fluent API
        //Add a unique index to the Game Player in the Fluent API
        //  modelBuilder.Entity<GamePlayer>()
        //      .HasIndex(c => new { c.PlayerID, c.GameID})
        //      .IsUnique();
        public int ID { get; set; }
        //public int TeamLineupID { get; set; }
        public TeamLineup TeamLineup { get; set; }
        public int GameID { get; set; }
        public Game Game { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }
        [Display(Name = "Lineup Batting Position")]
        public int BattingOrder { get; set; } = 0;
    }
}

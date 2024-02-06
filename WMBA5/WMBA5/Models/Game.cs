using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Game
    {
        public int ID { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "You must enter date and time for the Game.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.Today;


        [Required(ErrorMessage = "You cannot leave the Location blank.")]
        public int LocationID { get; set; }
        public Location Location { get; set; }

        [Display(Name = "Game Outcome")]
        //Win or lose, default to TBD (To be determined)
        public int OutcomeID { get; set; }
        public Outcome Outcome { get; set; }

        [Display(Name = "Division Name")]
        [Required(ErrorMessage = "You must select a Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
        public ICollection<Inning> Innings { get; set; } = new HashSet<Inning>();
        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();

        public TeamGame TeamGame { get; set; }
        public ICollection<TeamGame> TeamGames = new HashSet<TeamGame>();

    }
}

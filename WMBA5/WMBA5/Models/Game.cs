using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Game
    {
        public int ID { get; set; }

        [Display(Name ="Date")]
        [Required(ErrorMessage = "You must enter date and time for the Game.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "You cannot leave the Location blank.")]
        [StringLength(30, ErrorMessage = "Location cannot be more than 30 characters long.")]
        public string Location { get; set; }

        
        public string Outcome { get; set; } //Win or lose

        [Display(Name ="Division Name")]
        [Required(ErrorMessage = "You must select a Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
        public ICollection<Inning> Innings { get; set; } = new HashSet<Inning>();
        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();
    }
}

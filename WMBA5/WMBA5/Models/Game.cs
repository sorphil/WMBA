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

        [Required(ErrorMessage = "You cannot leave the Oponent blank.")]
        [StringLength(50, ErrorMessage = "Oponent cannot be more than 50 characters long.")]
        public string Oponent { get; set; }

        [Display(Name ="Playing At")]
        [Required(ErrorMessage = "You must specify if your team is playing Home or Away.")]
        public string PlayingAt { get; set; }
        public bool Outcome { get; set; } //Win or lose

        [Display(Name ="Division Name")]
        [Required(ErrorMessage = "You must select a Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }
    }
}

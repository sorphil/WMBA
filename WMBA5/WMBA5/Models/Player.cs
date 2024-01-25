using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Player
    {
        public int ID { get; set; }

        //Unique ID to identify the player by their Member ID
        [Display(Name = "Member ID")]
        [Required(ErrorMessage = "You cannot leave the Member ID blank.")]
        public string MemberID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cant leave the First Name blank")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(30, ErrorMessage = "Middle name cannot be more than 30 characters long.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Jersey Number")]
        public int JerseyNumber { get; set; }
        
       
        [Required(ErrorMessage = "You must enter a Birthday date and time for the Player.")]
        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }

        
        [Required(ErrorMessage = "You cannot leave the Position blank.")]
        [StringLength(30, ErrorMessage = "Position cannot be more than 30 characters long.")]
        public string Position { get; set; }

        //Foreign key
        [Display(Name = "Team Name")]
        //[Required(ErrorMessage = "You must select a Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        public int RosterID { get; set; }
        public Roster Roster { get; set; }

        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();
    }
}

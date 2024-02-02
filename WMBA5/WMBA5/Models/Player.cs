using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Player
    {
        #region Summary Properties

        [Display(Name = "Player")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName +(string.IsNullOrEmpty(Nickname) ? "" :
                        (" (" + (string)Nickname) + ") ");
            }
        }
        [Display(Name = "Player")]
        public string Summary
        {
            get
            {

                return MemberID + " - " + FirstName + " " + LastName
                    + (string.IsNullOrEmpty(Nickname) ? "" :
                        (" (" + (string)Nickname) + ") ")  ;
            }
        }
        #endregion
        public int ID { get; set; }

        //Unique ID to identify the player by their Member ID
        [Display(Name = "Member ID")]
        [Required(ErrorMessage = "You cannot leave the Member ID blank.")]
        public string MemberID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cant leave the First Name blank")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        
        [StringLength(30, ErrorMessage = "Nickname cannot be more than 30 characters long.")]
        public string Nickname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Jersey Number")]
        public int JerseyNumber { get; set; }
        
        


        public string Status { get; set; }
        //Foreign key

        [Display(Name = "Division")]
        [Required(ErrorMessage = "You must select a Divison")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Team Name")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        public int LineupID { get; set; }
        public Lineup Lineup { get; set; }

        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();
    }
}

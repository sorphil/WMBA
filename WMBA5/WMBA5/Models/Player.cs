using System.ComponentModel.DataAnnotations;
using WMBA5.Data;

namespace WMBA5.Models
{
    public class Player : IValidatableObject
    {
        #region Summary Properties

        [Display(Name = "Player")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName
                    + (string.IsNullOrEmpty(Nickname) ? "" :
                        (" (" + Nickname + ") "));
            }
        }
        [Display(Name = "Player")]
        public string Summary
        {
            get
            {

                return FirstName + ", " + LastName
                    + (string.IsNullOrEmpty(Nickname) ? "" :
                        (" (" + Nickname + ") "));
            }
        }
        #endregion
        public int ID { get; set; }

        //Unique ID to identify the player by their Member ID
        [Display(Name = "Member ID")]
        [Required(ErrorMessage = "You cannot leave the Member ID blank.")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$", ErrorMessage = "The Member ID must be at least 8 characters long, " +
            "it must have a combination of numbers and letters and it cant have symbols or special chracters(!,@,#,$,%,^,&,*)")]
        public string MemberID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cant leave the First Name blank")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Nickname")]
        [StringLength(30, ErrorMessage = "Nickname cannot be more than 30 characters long.")]
        public string Nickname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Range(0, 99, ErrorMessage = "Please enter a value between 1 and 99")]
        [Display(Name = "Jersey Number")]
        public int? JerseyNumber { get; set; } = null;

        //Status: Active or Inactive
        [Display(Name = "Status")]
        [Required(ErrorMessage = "You cannot leave the Status Blank")]
        public int StatusID { get; set; }
        [Display(Name ="Status")]
        public Status Status { get; set; }

        //Foreign key
        [Display(Name ="Division")]
        [Required(ErrorMessage = "You cannot leave the Division blank.")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Team Name")]
        public int? TeamID { get; set; }
        public Team Team { get; set; }

        
        //public int LineupID { get; set; }
        //public Lineup Lineup { get; set; }

        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();
        public ICollection<PlayerLineup> Lineups { get; set; } = new HashSet<PlayerLineup>();
        public ICollection<InGameStats> InGameStats { get; set; } = new HashSet<InGameStats>();
        //Adding validation for the jersey number
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Access the database context to check if the jersey number is already used by another player in the same team

            // Access the database context from dependency injection to check if the jersey number is already used by another player in the same team
            var dbContext = (WMBAContext)validationContext.GetService(typeof(WMBAContext));
            {
                var teamPlayerWithSameJersey = dbContext.Players
                    .Where(p => p.TeamID == TeamID && p.JerseyNumber == JerseyNumber && p.ID != ID)
                    .FirstOrDefault();

                if (teamPlayerWithSameJersey != null)
                {
                    yield return new ValidationResult("The jersey number you choose is already used by another player in the same team. Please choose a different jersey number.", new[] { "JerseyNumber" });
                }
                var player = dbContext.Players
                    .FirstOrDefault(p => p.ID == ID);

                var team = dbContext.Teams
                    .FirstOrDefault(t => t.ID == TeamID);

                if (player != null && team != null)
                {
                    var playerDivision = dbContext.Divisions
                    .FirstOrDefault(d => d.ID == player.DivisionID); ;
                    var teamDivision = dbContext.Divisions
                    .FirstOrDefault(d => d.ID == team.DivisionID);
                 
                    if (playerDivision != null && teamDivision != null)
                    {
                        List<string> validDivisions = new List<string> { "9U", "11U", "13U", "15U", "18U" };
                        // Get index of player and team divisions
                        int playerIndex = validDivisions.IndexOf(playerDivision.DivisionName);
                        int teamIndex = validDivisions.IndexOf(teamDivision.DivisionName);

                        // Check if the team's division is lower than the player's division
                        if (teamIndex < playerIndex)
                        {
                            yield return new ValidationResult("A player can only be assigned to a team with the same division or higher.", new[] { "DivisionID" });
                        }
                    }
                  
                }
               
            }
            //Adding validation so a player can olnly be in a team that is in its division or a superior Division
            //Ex: A U11 player can play in a U13 Team, but a U13 player cannot play in a U11 team
            //if (Division.ID > Team.Division.ID)
            //{
            //    yield return new ValidationResult("This player cant be in this division, the player is too old. Please choose a different division.", new[] { "TeamID" });
            //}
        }
    }
}

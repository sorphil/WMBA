using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
        [RegularExpression("^[A-Za-z0-9]{8,}$", ErrorMessage = "The Member ID must be at least 8 characters long, " +
            "it must have a combination of numbers and/or letters and it cant have symbols or special chracters(!,@,#,$,%,^,&,*,_,-)")]
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
        [DefaultValue("")]
        public int? StatusID { get; set; }
        public Status Status { get; set; }

        //Foreign key
        [Display(Name ="Division")]
        [DefaultValue("")]
        public int? DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Team Name")]
        [DefaultValue("")]
        public int? TeamID { get; set; }
        public Team Team { get; set; }

        
        //public int LineupID { get; set; }
        //public Lineup Lineup { get; set; }

        public ICollection<Stat> Stats { get; set; } = new HashSet<Stat>();

        //public ICollection<Game> Games { get; set; } 

        public ICollection<Score> Scores { get; set; } = new HashSet<Score>();

        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();
        //public ICollection<InGameStats> InGameStats { get; set; } = new HashSet<InGameStats>();

        [Display(Name = "Team Batting Position")]
        public int BattingOrder { get; set; } = 0;

        public ICollection<GamePlayer> GamePlayers { get; set; } = new HashSet<GamePlayer>();
        //Adding validation for the jersey number
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Access the database context to check if the jersey number is already used by another player in the same team

            // Access the database context from dependency injection to check if the jersey number is already used by another player in the same team
            var dbContext = (WMBAContext)validationContext.GetService(typeof(WMBAContext));
            {
                var teamPlayerWithSameJersey = dbContext.Players
                    .Where(p => p.TeamID == TeamID && p.JerseyNumber == JerseyNumber && p.ID != ID && p.JerseyNumber!= null)
                    .FirstOrDefault();

                if (teamPlayerWithSameJersey != null)
                {
                    yield return new ValidationResult("The jersey number you choose is already used by another player in the same team. Please choose a different jersey number.", new[] { "JerseyNumber" });
                }

                var originalPlayer = dbContext.Players.AsNoTracking().FirstOrDefault(p => p.ID == ID);

                // Check if the original division differs from the current division
                if (originalPlayer != null && originalPlayer.DivisionID != null)
                {
                    var originalDivisionID = originalPlayer.DivisionID;
                    var newDivisionID = (int?)DivisionID;

                    // Retrieve division names based on division IDs
                    var originalDivisionName = dbContext.Divisions.Where(d => d.ID == originalDivisionID).Select(d => d.DivisionName).FirstOrDefault();
                    var newDivisionName = dbContext.Divisions.Where(d => d.ID == newDivisionID).Select(d => d.DivisionName).FirstOrDefault();

                    // Check if both original and new division names are not null
                    if (!string.IsNullOrEmpty(originalDivisionName) && !string.IsNullOrEmpty(newDivisionName))
                    {
                        // Check if the original division is "13U" and the new division is "11U", or vice versa
                        if (originalDivisionName == "18U" && (newDivisionName == "15U" || newDivisionName == "13U" || newDivisionName == "11U" || newDivisionName == "9U"))
                        {
                            yield return new ValidationResult($"Players from '{originalDivisionName}' division cannot be changed to '{newDivisionName}' division.");
                        }

                        if (originalDivisionName == "15U" && (newDivisionName == "13U" || newDivisionName == "11U" || newDivisionName == "9U"))
                        {
                            yield return new ValidationResult($"Players from '{originalDivisionName}' division cannot be changed to '{newDivisionName}' division.", new[] {"DivisionID"});
                        }

                        if (originalDivisionName == "13U" && (newDivisionName == "11U" || newDivisionName == "9U" || newDivisionName == "18U"))
                        {
                            yield return new ValidationResult($"Players from '{originalDivisionName}' division cannot be changed to '{newDivisionName}' division.");
                        }

                        if (originalDivisionName == "11U" && (newDivisionName == "9U" || newDivisionName == "15U" || newDivisionName == "18U"))
                        {
                            yield return new ValidationResult($"Players from '{originalDivisionName}' division cannot be changed to '{newDivisionName}' division.");
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

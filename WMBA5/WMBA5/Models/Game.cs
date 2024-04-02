using System.ComponentModel.DataAnnotations;
using WMBA5.Data;

namespace WMBA5.Models
{
    public class Game :IValidatableObject
    {
        public int ID { get; set; }
        public string Summary
        {
            get
            {
                if (HomeTeam != null && AwayTeam != null)
                {
                    return HomeTeam?.TeamName + " vs " + AwayTeam?.TeamName + " - " + StartTime.ToString("dddd, MMMM dd, yyyy hh:mm tt");
                }
                else
                {
                    return StartTime.ToString("dddd, MMMM dd, yyyy hh:mm tt") + " (Teams not Loaded)";
                }

            }
        }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "You must enter date and time for the Game.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.Today;

        // Navigation properties
        [Display(Name = "Home Team")]
        [Required(ErrorMessage = "You must select the Home Team for the Game.")]
        public int HomeTeamID { get; set; }

        [Display(Name = "Home Team")]
        public Team HomeTeam { get; set; }

        [Display(Name = "Away Team")]
        [Required(ErrorMessage = "You must select the Away Team for the Game.")]
        public int AwayTeamID { get; set; }

        [Display(Name = "Away Team")]
        public Team AwayTeam { get; set; }


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


        [Display(Name = "Player At Bat")]
        public int? PlayerAtBatID { get; set; }
        public Player? PlayerAtBat { get; set; }


        public ICollection<Inning> Innings { get; set; } = new HashSet<Inning>();
        public ICollection<Score> Scores { get; set; } = new HashSet<Score>();

     

        [Display(Name = "Game Players")]
        public ICollection<GamePlayer> GamePlayers { get; set; } = new HashSet<GamePlayer>();

        public ICollection<Runner> Runners { get; set; } = new HashSet<Runner>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HomeTeamID == AwayTeamID)
            {
                yield return new ValidationResult("Home and Away teams must be different.", new[] { "AwayTeamID" });
            }
            
            if(StartTime < DateTime.Now)
            {
                yield return new ValidationResult("Game cannot be created back in time. Please select another date.", new[] { "StartTime" });
            }
            var basesWithMultiplePlayers = Runners
              .GroupBy(runner => runner.Base)
              .Where(group => group.Count() > 1)
              .Select(group => group.Key)
              .ToList();

            if (basesWithMultiplePlayers.Any())
            {
                yield return new ValidationResult($"Multiple players cannot be assigned to the same base: {string.Join(", ", basesWithMultiplePlayers)}.", new[] { "Runners" });
            }
        }

        
    }
}

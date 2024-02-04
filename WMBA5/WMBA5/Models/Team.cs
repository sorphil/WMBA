using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Team
    {
        public int ID { get; set; }

        [Display(Name = "Team Name")]
        [Required(ErrorMessage = "You cant leave the Team Name blank")]
        public string TeamName { get; set; }

        //Foreign keys
        //CoachID
        [Display(Name = "Coach Name")]
        [Required(ErrorMessage = "You must select a Coach")]
        public int CoachID { get; set; }
        public Coach Coach { get; set; }
        //DivisionID
        [Display(Name = "Division Name")]
        [Required(ErrorMessage = "You must select a Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }
        public int LineupID { get; set; }
        public Lineup Lineup { get; set; }

        public ICollection<Lineup> Lineups { get; set; } = new HashSet<Lineup>();

        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

		public ICollection<TeamGame> HomeTeamGames { get; set; } = new HashSet<TeamGame>();

		public ICollection<TeamGame> AwayTeamGames { get; set; } = new HashSet<TeamGame>();

	}
}

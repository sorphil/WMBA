using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMBA5.Models
{
    public class Team
    {
        #region Summary Properties
        //[Display(Name = "Team Name")]
        //public string TeamSummary
        //{
        //    get
        //    {
        //        return TeamName + " " + Division.DivisionName;
        //    }
        //}
        #endregion
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
        // Navigation properties
        [Display(Name = "Home Team Games")]
        [InverseProperty("HomeTeam")]
        public ICollection<Game> HomeGames { get; set; }

        [Display(Name = "Away Team Games")]
        [InverseProperty("AwayTeam")]
        public ICollection<Game> AwayGames { get; set; }

        //public ICollection<Game> Games { get; set; } = new HashSet<Game>();
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

    }
}

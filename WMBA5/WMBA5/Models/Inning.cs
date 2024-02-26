using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Inning
    {
        public int ID { get; set; }

       //Foreign Key
        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        public int GameScoreID { get; set; }
        public GameScore GameScore { get; set; }
        public int GameStatID { get; set; }
        public GameStat GameStat { get; set; }
    }
}

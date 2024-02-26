using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class PlayerScore
    {
        public int ID { get; set; }
        //Foreign Keys
        [Display(Name = "Player")]
        public int PlayerID { get; set; }
        public Player Player { get; set; }
        [Display(Name ="Score")]
        public int ScoreID { get; set; }
        public Score Score { get; set; }

        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }


    }
}

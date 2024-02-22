using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class PlayerGameScore
    {
        public int ID { get; set; }
        //Foreign Keys
        [Display(Name = "Player")]
        public int PlayerID { get; set; }
        public Player Player { get; set; }
        [Display(Name ="Game Score")]
        public int GameScoreID { get; set; }
        public GameScore GameScore { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class GameScore
    {
        public int ID { get; set; }
     
        //Foreign Keys
        [Display(Name ="Game")]
        public int GameID { get;set; }
        public Game Game { get; set; }


        public int PlayerAtBatID { get; set; }
        public PlayerAtBat PlayerAtBat { get; set; }

        public ICollection<PlayerScore> PlayerScores { get; set; } = new HashSet<PlayerScore>();


        

    }
}

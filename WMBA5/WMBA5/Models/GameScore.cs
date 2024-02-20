using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class GameScore
    {
        public int ID { get; set; }
        public int Inning { get; set; }
        public int Balls { get; set; }
        public int FoulBalls { get; set; }
        public int Strikes { get; set; }
        public int Out { get; set; }
        public int Runs { get; set; }
        public int Hits { get; set; }
        //Foreign Keys
        [Display(Name ="Game")]
        public int GameID { get;set; }
        public Game Game { get; set; }
        public int PlayerAtBatID { get; set; }
        public PlayerAtBat PlayerAtBat { get; set; }
        public ICollection<PlayerGameScore> PlayerGameScores { get; set; } = new HashSet<PlayerGameScore>();

    }
}

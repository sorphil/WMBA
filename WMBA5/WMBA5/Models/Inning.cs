using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Inning
    {
        public int ID { get; set; }

        public int Balls { get; set; }

        public int Hits { get; set; }
        public int Runs { get; set; }

        [Display(Name = "Foul Balls")]
        public int FoulBalls { get; set; }
        public int Strikes { get; set; }
        public int Out { get; set; }
        //Foreign Key
        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

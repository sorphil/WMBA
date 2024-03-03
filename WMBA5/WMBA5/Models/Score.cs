using WMBA5.Data;
namespace WMBA5.Models
{
    public class Score
    {
        public int ID { get; set; }
        public int Balls { get; set; }
        public int FoulBalls { get; set; }
        public int Strikes { get; set; }
        public int Out { get; set; }
        public int Runs { get; set; }
        public int Hits { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int InningID { get; set; }
        public Inning Inning { get; set; }

        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

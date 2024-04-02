using System.ComponentModel.DataAnnotations;
using WMBA5.Data;
namespace WMBA5.Models
{
    public class Score
    {
        [Display(Name = "Hits")]
        public int Hits
        {
            get
            {
                return Singles + Doubles + Triples;
            }
        }
        [Display(Name = "Outs")]
        public int Outs
        {
            get
            {
                return StrikeOuts + FlyOuts + GroundOuts;
            }
        }
        public int ID { get; set; }
        public int Balls { get; set; }
        public int FoulBalls { get; set; }
        public int Strikes { get; set; }
        public int Runs { get; set; }
        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int Triples { get; set; }
        public int StrikeOuts { get; set; }
        public int GroundOuts { get; set; }
        public int FlyOuts { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int InningID { get; set; }
        public Inning Inning { get; set; }

        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

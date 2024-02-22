using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class InGameStats
    {
        public int ID { get; set; }
        public int Balls { get; set; }
        public int Strikes { get; set; }    
        public int Out { get; set; }
        public int PlateApperance { get; set; }
        public int Runs { get; set; }
        public int RBI { get; set; }
        //Foreign Keys
        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }
        public int PlayerID { get; set; }
        public Player Player { get; set; }
    }
}

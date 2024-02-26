using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class GameStat
    {
        public int ID { get; set; } 

        //Foreign Keys
        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }
        [Display(Name = "Player Stats")]
        public ICollection<PlayerStat> PlayerStats { get; set; } = new HashSet<PlayerStat>();
    }
}

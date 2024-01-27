
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WMBA5.Models
{
    public class PlayerLineup
    {
        [Display(Name = "Player")]
        public int PlayerID { get; set; }
        public Player Player { get; set; }

        [Display(Name = "Lineup")]
        public int LineupID { get; set; }
        public Lineup Lineup { get; set; }
    }
}


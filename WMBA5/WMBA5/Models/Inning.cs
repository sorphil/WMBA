using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Inning
    {
        public int ID { get; set; }

        public string InningNo { get; set; }

        //Foreign Key
        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }


        public ICollection<Stat> Stats { get; set; } = new HashSet<Stat>();
        public ICollection<Score> Scores { get; set; } = new HashSet<Score>();



    }
}

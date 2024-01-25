using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Division
    {
        public int ID { get; set; }

        [Display(Name = "Division Name")]
        [Required(ErrorMessage = "You cant leave the Division Name blank")]
        public string DivisionName { get; set; }

        //Foreign key
        [Display(Name = "Club Name")]
        [Required(ErrorMessage = "You must select a Club")]
        public int ClubID { get; set; }
        public Club Club { get; set; }
        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}

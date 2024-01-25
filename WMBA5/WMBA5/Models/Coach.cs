using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Coach
    {
        public int ID { get; set; }

        [Display(Name = "Coach Name")]
        [Required(ErrorMessage = "You cant leave the Coach Name blank")]
        public string CoachName { get; set; }

        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
    }
}

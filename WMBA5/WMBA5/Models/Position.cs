using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Position
    {
        public int ID { get; set; }
        [Display(Name = "Position Name")]
        [Required(ErrorMessage = "You cant leave the Position Name blank")]
        [StringLength(50, ErrorMessage = "Position name cannot be more than 50 characters long.")]
        public string Name { get; set; }
    }
}

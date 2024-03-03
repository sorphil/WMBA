using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Status
    {

        public int ID { get; set; }
        [Display(Name = "Status")]
        public string StatusName { get; set; }
    }
}

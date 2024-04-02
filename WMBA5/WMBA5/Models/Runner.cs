using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
    public class Runner
    {
        public int ID { get; set; }
        public int? PlayerID { get; set; }
        public Player? Player { get; set; }
        public int GameID { get; set; }
        public Game Game { get; set; }

        public Base Base { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Check if the base is not within the valid range (1, 2, or 3)
            if (Base < Base.First || Base > Base.Third)
            {
                yield return new ValidationResult("Invalid base value. Base must be either First, Second, or Third.", new[] { "Base" });
            }
        }
    }
}

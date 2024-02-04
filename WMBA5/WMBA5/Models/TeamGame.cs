using System.ComponentModel.DataAnnotations;

namespace WMBA5.Models
{
	public class TeamGame : IValidatableObject
	{
		public int Id { get; set; }

		[Display(Name = "Home Team")]
		//[Required(ErrorMessage = "Home Team is required")]
		[Range(1, int.MaxValue, ErrorMessage = "Home Team is required")]
		public int HomeTeamID { get; set; }
		public Team HomeTeam { get; set; }

		[Display(Name = "Away Team")]
		//[Required(ErrorMessage = "Away Team is required")]
		[Range(1, int.MaxValue, ErrorMessage = "Away Team is required")]
		public int AwayTeamID { get; set; }
		public Team AwayTeam { get; set; }

		[Display(Name = "Game")]
		public int GameID { get; set; }
		public Game Game { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (HomeTeamID == AwayTeamID)
			{
				yield return new ValidationResult("Home Team and Away Team cannot be the same.", new[] { nameof(AwayTeamID) });
			}
		}
	}
}

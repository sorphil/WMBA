namespace WMBA5.Models
{
    public class Lineup
    {
        public int ID { get; set; }

        //Foreign Keys
        public int GameID { get; set; }
        public Game Game { get; set; }

        public int TeamID { get; set; }
        public Team Team { get; set; }

        //public ICollection<PlayerLineup> Lineups { get; set; } = new HashSet<PlayerLineup>();

    }
}

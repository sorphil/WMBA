namespace WMBA5.Models
{
    public class Inning
    {
        public int ID { get; set; }

        public int InningNumber { get; set; }

        public int RunsScored { get; set; }

        public int PlayersOut { get; set; }

        public ICollection<PlayerAtBat> PlayerAtBats { get; set; } = new HashSet<PlayerAtBat>();

        //Foreign Key
        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

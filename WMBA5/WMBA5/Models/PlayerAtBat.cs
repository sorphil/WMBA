namespace WMBA5.Models
{
    public class PlayerAtBat
    {
        public int ID { get; set; }

        public string Result { get; set; }

        //Foreign Keys
        public int GameID { get; set; }
        public Game Game { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int InningID { get; set; }
        public Inning Inning { get; set; }
    }
}

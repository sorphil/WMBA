namespace WMBA5.Models
{
    public class Runner
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public Player Player { get; set; }
        public int GameID { get; set; }
        public Game Game { get; set; }

        public Base Base { get; set; }
    }
}

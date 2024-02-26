namespace WMBA5.Models
{
    public class PlayerStat
    {
        public int ID { get; set; }


        //Foreign Key
        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int StatID { get; set; }
        public Stat Stat { get; set; }

        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

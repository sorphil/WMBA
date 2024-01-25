namespace WMBA5.Models
{
    public class PlayerStat
    {
        public int ID { get; set; }

        public int GamesPlayed { get; set; }

        public int PlayerApperance { get; set; }

        public int Hits { get; set; }

        public int RunsScored { get; set; }

        public int StrikeOuts { get; set; }

        public int Walks {  get; set; }

        public int RBI { get; set; }

        //Foreign Key
        public int PlayerID { get; set; }
        public Player Player { get; set; }
    }
}

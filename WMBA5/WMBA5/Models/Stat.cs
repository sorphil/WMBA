using WMBA5.Data;
namespace WMBA5.Models
{
    public class Stat
    {
        public int ID { get; set; }
        public int GamesPlayed { get; set; }

        public int PlayerAppearance { get; set; }

        public int Hits { get; set; }

        public int RunsScored { get; set; }

        public int StrikeOuts { get; set; }

        public int Walks { get; set; }

        public int RBI { get; set; }

        //Foreign Key
        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int InningID { get; set; }
        public Inning Inning { get; set; }

        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}

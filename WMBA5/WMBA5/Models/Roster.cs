namespace WMBA5.Models
{
    public class Roster
    {
        public int ID { get; set; }

        public bool IsPlaying { get; set; }

        public string BattingOrder { get; set; }

        public string FieldingPosition { get; set; }

        //Foreign Keys
        public int GameID { get; set; }
        public Game Game { get; set; }

        public int TeamID { get; set; }
        public Team Team { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }
        
    }
}

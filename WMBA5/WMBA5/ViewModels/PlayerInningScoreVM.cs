namespace WMBA5.ViewModels
{
    public class PlayerInningScoreVM
    {
        public int ID { get; set; }
        public int InningID { get; set; }
        public int GameID { get; set; }
        public int PlayerID { get; set; }
        public int TotalBalls { get; set; }
        public int TotalFoulBalls { get; set; }
        public int TotalStrikes { get; set; }
        public int TotalHits { get; set; }
        public int TotalOuts { get; set; }

        public int TotalRuns { get; set; }
    }
}

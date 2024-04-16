namespace WMBA5.ViewModels
{
    public class PlayerScoresStatsVM
    {
        public int ID { get; set; }
        public int GameID { get; set; }
        public int PlayerID { get; set; }
        public int TotalGamesPlayed { get; set; }

        public int TotalPlayerAppearances { get; set; }

        public int TotalHits { get; set; }

        public int TotalRunsScored { get; set; }

        public int TotalStrikeOuts { get; set; }

        public int TotalWalks { get; set; }

        public int TotalRBI { get; set; }

        // Additional properties from the Score table
        public int TotalBalls { get; set; }
        public int TotalFoulBalls { get; set; }
        public int TotalStrikes { get; set; }
        public int TotalOut { get; set; }
        public int TotalRuns { get; set; }
    }
}

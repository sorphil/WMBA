namespace WMBA5.ViewModels
{
    public class GameStatsVM
    {
        public int GameID { get; set; }
        public int PlayerID { get; set; }
        public int TotalGamesPlayed { get; set; }

        public int TotalPlayerAppearances { get; set; }

        public int TotalHits { get; set; }

        public int TotalRunsScored { get; set; }

        public int TotalStrikeOuts { get; set; }

        public int TotalWalks { get; set; }

        public int TotalRBI { get; set; }
    }
}

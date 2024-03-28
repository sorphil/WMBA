using System.ComponentModel.DataAnnotations;
using WMBA5.Data;
namespace WMBA5.Models
{
    public class Stat
    {
        public int ID { get; set; }

        #region calculated stats
        [Display(Name = "Batting/AVG")]
        public decimal BattAVG
        {
            get
            {
                return Hits / PlayerAppearance;
            }
        }
        //[Display(Name = "Slugging Percentage ")]
        //public decimal SLG
        //{
        //    get
        //    {
        //        return (single + double*2 + triple*3 + HR*4)/ PlayerAppearance;
        //    }
        //}
        #endregion

        [Display(Name = "Games Played")]
        public int GamesPlayed { get; set; }

        [Display(Name = "Player Appearances")]
        public int PlayerAppearance { get; set; }

        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int  Triples { get; set; }
        public int StrikeOut { get; set; }
        public int GroundOut { get; set; }
        public int PopOut { get; set; }

        [Display(Name = "Runs Scored")]
        public int RunsScored { get; set; }

        [Display(Name = "Strike Outs")]
        public int StrikeOuts { get; set; }

        [Display(Name = "Walks")]
        public int Walks { get; set; }

        [Display(Name = "RBI")]
        public int RBI { get; set; }

        // Foreign Key
        [Display(Name = "Player")]
        public int PlayerID { get; set; }

        [Display(Name = "Player")]
        public Player Player { get; set; }
    }
}

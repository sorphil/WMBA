﻿using System.ComponentModel.DataAnnotations;
using WMBA5.Data;
namespace WMBA5.Models
{
    public class Stat
    {
        public int ID { get; set; }

        [Display(Name = "Games Played")]
        public int GamesPlayed { get; set; }

        [Display(Name = "Player Appearances")]
        public int PlayerAppearance { get; set; }

        public int Hits { get; set; }

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
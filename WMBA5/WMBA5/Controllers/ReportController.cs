using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;
using WMBA5.Utilities;
using WMBA5.ViewModels;


namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
    public class ReportController : CognizantController
    {
        private readonly WMBAContext _context;



        public ReportController(WMBAContext context)
        {
            _context = context;
        }


        public IActionResult GameReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stats = from a in _context.Scores
                        .Include(a => a.Game)
                        .Include(a => a.Inning)
                        .OrderBy(a => a.Game.HomeTeam)
                        select new
                        {
                            Game = a.Game.HomeTeam.TeamName + " " + a.Game.AwayTeam.TeamName,
                            Name = a.Player.FullName,
                            a.Runs,
                            a.Balls,
                            a.FoulBalls,
                            a.Strikes,
                            //a.Out,
                            a.Hits
                        };

            int numRows = stats.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("GameReport");

                    workSheet.Cells[3,2].LoadFromCollection(stats, true);

                    using (ExcelRange headings = workSheet.Cells[3,2,3,9])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                    }
                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1,2].Value = "Game Report";
                    using (ExcelRange Rng = workSheet.Cells[1,2,1,9])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 19;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "GameReport.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No Data");
        }

        public async Task<IActionResult> PlayerStatsView(int? GameID,int? page, int? pageSizeID)
        {
            var sumQ = _context.PlayerInningScoreSummary
              .AsNoTracking();


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "DownloadReport");//Remember for this View
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<PlayerInningScoreVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            PopulateDropDownLists(GameID, null, null);
            return View(pagedData);
        }
        //Playern Inning score summary Download
        public IActionResult PlayerStatDownload(int ? GameID)
        {
           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string feedBack = "";

            string homeTeam = _context.Games.FirstOrDefault(g => g.ID == GameID)?.HomeTeam.TeamName;
            string awayTeam = _context.Games.FirstOrDefault(g => g.ID == GameID)?.AwayTeam.TeamName;
            string gameResult = _context.Games.FirstOrDefault(g => g.ID == GameID)?.Outcome.OutcomeString;

            var stats2 = _context.PlayerInningScoreSummary
                        .OrderBy(a => a.PlayerID)
                        .Where(a => a.GameID == GameID)
                        .Select(a => new
                        {
                            Game = homeTeam + " VS " + awayTeam,
                            Name = _context.Players.FirstOrDefault(p => p.ID == a.PlayerID).FullName,
                            Result = gameResult,
                            a.TotalRuns,
                            a.TotalBalls,
                            a.TotalFoulBalls,
                            a.TotalStrikes,
                            a.TotalOuts,
                            a.TotalHits
                        })
                        .AsNoTracking();

            int numRows = stats2.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    DateTime localDate = DateTime.Now;
                    var workSheet = excel.Workbook.Worksheets.Add("PlayerStatsReport");

                    workSheet.Cells[3, 2].LoadFromCollection(stats2, true);
                    workSheet.Cells[2, 11].Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    using (ExcelRange headings = workSheet.Cells[3, 2, 3, 11])
                    {
                        headings.Style.Font.Bold = true;
                    }
                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 2].Value = "Player Stats Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 11])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 19;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    workSheet.Cells.AutoFitColumns();

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "PlayerStatsReport.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }

            return NotFound("No data.");
        }

        //public IActionResult PlayerStatReport()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    var stats = from a in _context.Scores
        //                .Include(a => a.Game)
        //                .Include(a => a.Player)
        //                .OrderBy(a => a.Player.LastName)
        //                .Where(a => a.GameID == GameID)
        //                select new
        //                {
        //                    Game = a.Game.HomeTeam.TeamName + " " + a.Game.AwayTeam.TeamName,
        //                    Name = a.Player.FullName,
        //                    a.Runs,
        //                    a.Balls,
        //                    a.FoulBalls,
        //                    a.Strikes,
        //                    a.Out,
        //                    a.Hits
        //                };

        //    int numRows = stats.Count();

        //    if (numRows > 0)
        //    {
        //        using (ExcelPackage excel = new ExcelPackage())
        //        {
        //            var workSheet = excel.Workbook.Worksheets.Add("PlayerStatsReport");

        //            workSheet.Cells[3, 2].LoadFromCollection(stats, true);

        //            using (ExcelRange headings = workSheet.Cells[3, 2, 3, 9])
        //            {
        //                headings.Style.Font.Bold = true;
        //                var fill = headings.Style.Fill;
        //                fill.PatternType = ExcelFillStyle.Solid;
        //                fill.BackgroundColor.SetColor(Color.LightBlue);
        //            }
        //            workSheet.Cells.AutoFitColumns();

        //            workSheet.Cells[1, 2].Value = "Player Stats Report";
        //            using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 9])
        //            {
        //                Rng.Merge = true;
        //                Rng.Style.Font.Bold = true;
        //                Rng.Style.Font.Size = 19;
        //                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            }

        //            try
        //            {
        //                Byte[] theData = excel.GetAsByteArray();
        //                string filename = "PlayerStatsReport.xlsx";
        //                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
        //                return File(theData, mimeType, filename);
        //            }
        //            catch
        //            {
        //                return BadRequest("Could not build and download the file.");
        //            }
        //        }
        //    }
        //    return NotFound("No Data");
        //}

        //public IActionResult BattingAnalysisReport()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    string feedBack = "";
        //    var stats = from a in _context.Stats
        //                .Include(a => a.Player)
        //                .OrderBy(a => a.Player.LastName)
        //                select new
        //                {
        //                    Name = a.Player.FullName,
        //                    at_bats = a.PlayerAppearance,
        //                    a.Hits,
        //                    a.RunsScored,
        //                    a.StrikeOuts,
        //                    a.Walks,
        //                    a.RBI
        //                };

        //    int numRows = stats.Count();

        //    if (numRows > 0)
        //    {
        //        using (ExcelPackage excel = new ExcelPackage())
        //        {
        //            var workSheet = excel.Workbook.Worksheets.Add("PlayerStatsReport");

        //            workSheet.Cells[3, 2].LoadFromCollection(stats, true);

        //            using (ExcelRange headings = workSheet.Cells[3, 2, 3, 8])
        //            {
        //                headings.Style.Font.Bold = true;
        //                var fill = headings.Style.Fill;
        //                fill.PatternType = ExcelFillStyle.Solid;
        //                fill.BackgroundColor.SetColor(Color.LightBlue);
        //            }
        //            workSheet.Cells.AutoFitColumns();

        //            workSheet.Cells[1, 2].Value = "Player Stats Report";
        //            using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 8])
        //            {
        //                Rng.Merge = true;
        //                Rng.Style.Font.Bold = true;
        //                Rng.Style.Font.Size = 19;
        //                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            }

        //            try
        //            {
        //                Byte[] theData = excel.GetAsByteArray();
        //                string filename = "PlayerStatsReport.xlsx";
        //                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
        //                return File(theData, mimeType, filename);
        //            }
        //            catch
        //            {
        //                return BadRequest("Could not build and download the file.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        feedBack = "No data to download";
        //    }
        //    TempData["Feedback"] = feedBack;
        //    return View(stats);
        //}
        public async Task<IActionResult> PlayerComparisionView(int? playerID1, int? playerID2, int? page, int? pageSizeID)
        {
            var sumQ = _context.PlayerScoreStatsSummary
              .AsNoTracking();


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "DownloadReport");//Remember for this View
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<PlayerScoresStatsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            PopulateDropDownLists(null, playerID1, playerID2);
            return View(pagedData);
        }
        public async Task<IActionResult> PlayerComparisionDownload(int? PlayerID1, int? PlayerID2, string? MemberID)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string feedBack = "";
            PopulateDropDownLists(null, PlayerID1, PlayerID2);

            string playerName = _context.Players.FirstOrDefault(p => p.ID == PlayerID1)?.FullName;

            var statsp1 = _context.PlayerScoreStatsSummary
                        .OrderBy(a => a.PlayerID)
                        .Where(a => a.PlayerID == PlayerID1)
                        .Select(a => new
                        {
                            Name = playerName,
                            At_Batts = a.TotalGamesPlayed,
                            a.TotalHits,
                            a.TotalRunsScored,
                            a.TotalStrikeOuts,
                            a.TotalWalks,
                            a.TotalRBI,
                            a.TotalBalls,
                            a.TotalFoulBalls,
                            a.TotalStrikes,
                            a.TotalOut,
                            a.TotalRuns
                        });


            string playerName2 = _context.Players.FirstOrDefault(p => p.ID == PlayerID1)?.FullName;

            var statsp2 = _context.PlayerScoreStatsSummary
                        .OrderBy(a => a.PlayerID)
                        .Where(a => a.PlayerID == PlayerID2)
                        .Select(a => new
                        {
                            Name = playerName2,
                            At_Batts = a.TotalGamesPlayed,
                            a.TotalHits,
                            a.TotalRunsScored,
                            a.TotalStrikeOuts,
                            a.TotalWalks,
                            a.TotalRBI,
                            a.TotalBalls,
                            a.TotalFoulBalls,
                            a.TotalStrikes,
                            a.TotalOut,
                            a.TotalRuns
                        });

            int numRows = statsp2.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    DateTime localDate = DateTime.Now;
                    var workSheet = excel.Workbook.Worksheets.Add("PlayerStatsReport");

                    workSheet.Cells[3, 2].LoadFromCollection(statsp1, true);
                    workSheet.Cells[6, 2].LoadFromCollection(statsp2, true);
                    workSheet.Cells[2, 11].Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    using (ExcelRange headings = workSheet.Cells[3, 2, 3, 11])
                    {
                        headings.Style.Font.Bold = true;
                    }
                    workSheet.Cells.AutoFitColumns();
                    using (ExcelRange headings = workSheet.Cells[6, 2, 6, 11])
                    {
                        headings.Style.Font.Bold = true;
                    }
                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 2].Value = "Player Stats Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 11])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 19;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "PlayerStatsReport.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            else
            {
                feedBack = "No data to download, Please select a game already played";
            }
            TempData["Feedback"] = feedBack;
            return NotFound("No data.");
        }
        //public IActionResult PlayerComparisionReport()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    string feedBack = "";
        //    var stats = from a in _context.Stats
        //                .Include(a => a.Player)
        //                .OrderBy(a => a.Player.LastName)
        //                .Where(a => a.Player.MemberID == "FE9113FF")
        //                select new
        //                {
        //                    Name = a.Player.FullName,
        //                    At_Batts = a.PlayerAppearance,
        //                    a.RunsScored,
        //                    a.StrikeOuts,
        //                    a.Walks,
        //                    a.Hits,
        //                    a.RBI
        //                };

        //    var stats2 = from a in _context.Stats
        //    .Include(a => a.Player)
        //    .OrderBy(a => a.Player.LastName)
        //    .Where(a => a.Player.MemberID == "2C5E0779")
        //                select new
        //                {
        //                    Name = a.Player.FullName,
        //                    At_Batts = a.PlayerAppearance,
        //                    a.RunsScored,
        //                    a.StrikeOuts,
        //                    a.Walks,
        //                    a.Hits,
        //                    a.RBI
        //                };

        //    int numRows = stats.Count();

        //    if (numRows > 0)
        //    {
        //        using (ExcelPackage excel = new ExcelPackage())
        //        {
        //            var workSheet = excel.Workbook.Worksheets.Add("PlayerComparisionReport");

        //            workSheet.Cells[3, 2].LoadFromCollection(stats, true);

        //            workSheet.Cells[5, 2].LoadFromCollection(stats2, true);

        //            using (ExcelRange headings = workSheet.Cells[3, 2, 3, 9])
        //            {
        //                headings.Style.Font.Bold = true;
        //                var fill = headings.Style.Fill;
        //                fill.PatternType = ExcelFillStyle.Solid;
        //            }
        //            using (ExcelRange headings = workSheet.Cells[5, 2, 5, 9])
        //            {
        //                headings.Style.Font.Bold = true;
        //                var fill = headings.Style.Fill;
        //                fill.PatternType = ExcelFillStyle.Solid;
        //            }
        //            workSheet.Cells.AutoFitColumns();

        //            workSheet.Cells[1, 2].Value = "Player Comparison Report";
        //            using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 9])
        //            {
        //                Rng.Merge = true;
        //                Rng.Style.Font.Bold = true;
        //                Rng.Style.Font.Size = 19;
        //                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            }

        //            try
        //            {
        //                Byte[] theData = excel.GetAsByteArray();
        //                string filename = "PlayerComparisionReport.xlsx";
        //                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
        //                return File(theData, mimeType, filename);
        //            }
        //            catch
        //            {
        //                return BadRequest("Could not build and download the file.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        feedBack = "No data to download";
        //    }
        //    TempData["Feedback"] = feedBack;
        //    return View(stats);
        //}
        public IActionResult TeamComparisionReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stats = from a in _context.Stats
                        .Include(a => a.Player)
                        .OrderBy(a => a.Player.LastName)
                        select new
                        {
                            Name = a.Player.FullName,
                            at_bats = a.PlayerAppearance,
                            a.Hits,
                            a.RunsScored,
                            a.StrikeOuts,
                            a.Walks,
                            a.RBI
                        };

            int numRows = stats.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("PlayerStatsReport");

                    workSheet.Cells[3, 2].LoadFromCollection(stats, true);

                    using (ExcelRange headings = workSheet.Cells[3, 2, 3, 8])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                    }
                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 2].Value = "Player Stats Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 2, 1, 8])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 19;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "PlayerStatsReport.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.Sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No Data");

        }

        public IActionResult DownloadStats()
        {
            //Get the players

            var playerStats = _context.Divisions
                .Include(d => d.Teams)
                .Include(d => d.Players)
                .ThenInclude(d => d.Stats)
                .AsNoTracking()
                .ToList();


            if (playerStats.Any())

            {
                //Create a new spreadsheet from scratch
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Stats");

                    int rowNum = 3;

                    foreach (var division in playerStats)
                    {
                        rowNum++;


                        workSheet.Cells[rowNum, 1].Value = "Division: " + division.DivisionName;
                        workSheet.Cells[rowNum, 1].Style.Font.Bold = true;
                        workSheet.Cells[rowNum, 1, rowNum, 11].Merge = true;

                        workSheet.Cells[rowNum, 1, rowNum, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                        //workSheet.Cells[3, 1, rowNum, 11].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black);

                        rowNum++;

                        foreach (var team in division.Teams)
                        {
                            workSheet.Cells[rowNum, 1].Value = "Team: " + team.TeamName;
                            workSheet.Cells[rowNum, 1].Style.Font.Bold = true;

                            workSheet.Cells[rowNum, 1, rowNum, 11].Merge = true;
                            workSheet.Cells[rowNum, 1, rowNum, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                            //workSheet.Cells[3, 1, rowNum, 11].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black);

                            rowNum++;

                            var players = _context.Players.Include(p => p.Stats).Include(p => p.Scores).Where(p => p.TeamID == team.ID).AsNoTracking().ToList();
                            foreach (var player in players)
                            {
                                workSheet.Cells[rowNum, 1].Value = player.FullName;
                                foreach (var stat in player.Stats)
                                {
                                    workSheet.Cells[rowNum, 2].Value = stat.GamesPlayed != 0 ? stat.GamesPlayed : "0";
                                    workSheet.Cells[rowNum, 3].Value = stat.PlayerAppearance != 0 ? stat.PlayerAppearance : "0";
                                    workSheet.Cells[rowNum, 4].Value = stat.RunsScored != 0 ? stat.RunsScored : "0";
                                    workSheet.Cells[rowNum, 5].Value = stat.Hits != 0 ? stat.Hits : "0";

                                    foreach(var score in player.Scores)
                                    {
                                        workSheet.Cells[rowNum, 6].Value = score.Singles != 0 ? score.Singles : "0" ;//needs some fixes in the model
                                        workSheet.Cells[rowNum, 7].Value = score.Doubles != 0 ? score.Doubles : "0";//needs some fixes in the model
                                        workSheet.Cells[rowNum, 8].Value = score.Triples != 0 ? score.Triples : "0";//needs some fixes in the model
                                    }
                                    workSheet.Cells[rowNum, 9].Value = stat.RBI != 0 ? stat.RBI : "0";
                                    workSheet.Cells[rowNum, 10].Value = stat.Outs != 0 ? stat.Outs : "0";
                                    workSheet.Cells[rowNum, 11].Value = stat.BattAVG != 0 ? stat.BattAVG : "0";
                                    //workSheet.Cells[rowNum, 12].Value = ((int.Parse(workSheet.Cells[rowNum, 6].Text) + (int.Parse(workSheet.Cells[rowNum, 7].Text) * 2) + (int.Parse(workSheet.Cells[rowNum, 8].Text) * 3))/ stat.GamesPlayed);

                                }

                                rowNum++;


                            }
                            //workSheet.Cells[3, 1, rowNum, 11].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black);
                            rowNum++;
                        }


                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;

                        //workSheet.Cells[3, 1, rowNum, 11].Style.Border.BorderAround(ExcelBorderStyle.Hair, Color.Black);
                        //workSheet.Cells[3, 1, 3, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[3, 1, rowNum, 11].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                        //workSheet.Cells[3, 1, rowNum, 11].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                    }

                    workSheet.Cells.AutoFitColumns();

                    DateTime localDate = DateTime.Now;
                    //TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone).ToLocalTime();

                    workSheet.Cells[1, 1].Value = "Player Stats";
                    workSheet.Cells[1, 1, 1, 12].Merge = true; //need to merge more cells
                    workSheet.Cells[1, 1, 1, 12].Style.Font.Bold = true;
                    workSheet.Cells[1, 1, 1, 12].Style.Font.Size = 18;
                    workSheet.Cells[1, 1, 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    workSheet.Cells[2, 1].Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[2, 12].Style.Font.Bold = true;
                    workSheet.Cells[2, 12].Style.Font.Size = 12;
                    workSheet.Cells[2, 1, 2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    //workSheet headings
                    workSheet.Cells[3, 2].Value = "Games Played";
                    workSheet.Cells[3, 3].Value = "Plate Apperances";
                    workSheet.Cells[3, 4].Value = "Runs Scored";
                    workSheet.Cells[3, 5].Value = "Hits";
                    workSheet.Cells[3, 6].Value = "Singles";
                    workSheet.Cells[3, 7].Value = "Doubles";
                    workSheet.Cells[3, 8].Value = "Triples";
                    workSheet.Cells[3, 9].Value = "RBI";
                    workSheet.Cells[3, 10].Value = "Outs";
                    workSheet.Cells[3, 11].Value = "Batting AVG";
                    workSheet.Cells[3, 12].Value = "SLG";


                    //borders
                    //workSheet.Cells[3, 1, 3, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //workSheet.Cells[3, 1, 3, 11].Style.Border.BorderAround.Style = ExcelBorderStyle.Thick;


                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();

                        string filename = "BattingAnalysis.xlsx";

                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Could not build and download the file.\n" +
                            "Reason: " + ex.Message);
                    }
                }
            }
            return View(playerStats);
        }



        private SelectList GameSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .OrderBy(m => m.StartTime), "ID", "Summary", selectedId);
        }
        private SelectList DivisionSelectionList(int? selectedDivisionId)
        {
            var divisions = _context.Divisions
                .OrderBy(d => d.DivisionName)
                .ToList();

            return new SelectList(divisions, "ID", "DivisionName", selectedDivisionId);
        }
        private SelectList PlayerSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Players
                .Include(p => p.Stats)
                .OrderBy(p => p.LastName), "ID", "FullName", selectedId);
        }
        private void PopulateDropDownLists(int? ID, int? playerID1, int? playerID2)
        {
            //ViewData["DivisionID"] = DivisionSelectionList(stat?.Player?.Division?.ID);
            ViewData["GameID"] = GameSelectionList(ID);
            ViewData["PlayerID1"] = PlayerSelectionList(playerID1);
            ViewData["PlayerID2"] = PlayerSelectionList(playerID2);
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Runtime.CompilerServices;
using WMBA5.CustomControllers;
using WMBA5.Data;

namespace WMBA5.Controllers
{
    public class ReportController : CognizantController
    {
        private readonly WMBAContext _context;

        public ReportController(WMBAContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

            if (playerStats .Any())
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
                        workSheet.Cells[rowNum, 1 , rowNum, 11].Merge = true;
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
                            workSheet.Cells[rowNum, 1 , rowNum, 11].Merge = true;
                            workSheet.Cells[rowNum, 1 , rowNum , 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            //workSheet.Cells[rowNum, 1, rowNum, 11].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                            //workSheet.Cells[3, 1, rowNum, 11].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black);

                            rowNum++;

                            var players = _context.Players.Include(p => p.Stats).Where(p => p.TeamID == team.ID).AsNoTracking().ToList();
                            foreach (var player in players)
                            {
                                workSheet.Cells[rowNum, 1].Value = player.FullName;
                                
                                foreach(var stat in player.Stats)
                                {
                                    workSheet.Cells[rowNum, 2].Value = stat.GamesPlayed != 0 ? stat.GamesPlayed : "0";
                                    workSheet.Cells[rowNum, 3].Value = stat.PlayerAppearance != 0 ? stat.PlayerAppearance : "0";
                                    workSheet.Cells[rowNum, 4].Value = stat.RunsScored != 0 ? stat.RunsScored : "0";
                                    workSheet.Cells[rowNum, 5].Value = stat.Hits != 0 ? stat.Hits : "0";
                                    workSheet.Cells[rowNum, 6].Value = "0";       //stat.Single;//needs some fixes in the model
                                    workSheet.Cells[rowNum, 7].Value = "0";       //stat.Double;//needs some fixes in the model
                                    workSheet.Cells[rowNum, 8].Value = "0";       //stat.Triple;//needs some fixes in the model
                                    workSheet.Cells[rowNum, 9].Value = "0";       //stat.HomeRuns//needs some fixes in the model
                                    workSheet.Cells[rowNum, 10].Value = stat.RBI != 0 ? stat.RBI : "0";
                                    workSheet.Cells[rowNum, 11].Value = stat.StrikeOuts != 0 ? stat.StrikeOuts : "0";
                                    
                                }

                                rowNum ++;
                                
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
                    workSheet.Cells[1, 1, 1, 11].Merge = true; //need to merge more cells
                    workSheet.Cells[1, 1, 1, 11].Style.Font.Bold = true;
                    workSheet.Cells[1, 1, 1, 11].Style.Font.Size = 18;
                    workSheet.Cells[1, 1, 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    workSheet.Cells[2, 1].Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    workSheet.Cells[2, 1, 2, 11].Merge = true;
                    workSheet.Cells[2, 11].Style.Font.Bold = true;
                    workSheet.Cells[2, 11].Style.Font.Size = 12;
                    workSheet.Cells[2, 1, 2, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    //workSheet headings
                    workSheet.Cells[3, 2].Value = "Games Played";
                    workSheet.Cells[3, 3].Value = "Plate Apperances";
                    workSheet.Cells[3, 4].Value = "Runs Scored";
                    workSheet.Cells[3, 5].Value = "Hits";
                    workSheet.Cells[3, 6].Value = "Singles";
                    workSheet.Cells[3, 7].Value = "Doubles";
                    workSheet.Cells[3, 8].Value = "Triples";
                    workSheet.Cells[3, 9].Value = "HomeRuns";
                    workSheet.Cells[3, 10].Value = "RBI";
                    workSheet.Cells[3, 11].Value = "StrikeOuts";

                    //borders
                    //workSheet.Cells[3, 1, 3, 11].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //workSheet.Cells[3, 1, 3, 11].Style.Border.BorderAround.Style = ExcelBorderStyle.Thick;


                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "PlayerStats.xlsx";
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
            else
            {
                return NotFound("No data.");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using WMBA5.Data;
using WMBA5.ViewModels;
using WMBA5.Models;
using Microsoft.VisualBasic.FileIO;
using WMBA5.Utilities;
using ImportReport = WMBA5.ViewModels.ImportReport;

namespace WMBA5.Controllers
{
    public class ImportReportsController : Controller
    {
        private readonly WMBAContext _context;

        public ImportReportsController(WMBAContext context)
        {
            _context = context;
        }

        public void ImportTeams(List<ImportReport> imported, string feedBack)
        {
            List<Team> teams = new List<Team>();
            foreach (ImportReport r in imported)
            {
                string teamDivision = r.Division + r.Team;
                string existingTeam = _context.Divisions.FirstOrDefault(t => t.DivisionName == r.Division)?.DivisionName + _context.Teams.FirstOrDefault(t => t.TeamName == r.Team)?.TeamName;
                if(existingTeam != teamDivision)
                {
                    Team t = new Team
                    {
                        TeamName = r.Team,
                        DivisionID = _context.Divisions.FirstOrDefault(d => d.DivisionName == r.Division).ID
                    };
                    teams.Add(t);
                }

            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Teams.AddRange(teams);
                    _context.SaveChanges();
                    transaction.Commit();
                    if (feedBack == "")
                    {
                        feedBack = "Teams added successfully.";
                    }
                    else if (teams.Count > 0)
                    {
                        feedBack = "Teams have been added if they were not already in the database.";
                    }
                    else
                    {
                        feedBack = "Teams already exists in the database. No teams were added.";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    feedBack = $"Error: An error occurred while saving data. {ex.Message}";
                }
            }
        }
        public void ImportPlayers(List<ImportReport> imported, string feedBack)
        {
            List<Player> players = new List<Player>();
            foreach (ImportReport r in imported)
            {
                int teamID = _context.Teams.FirstOrDefault(t => t.TeamName == r.Team).ID;
                int divisionID = _context.Divisions.FirstOrDefault(d => d.DivisionName==r.Division).ID;
                int statusID = _context.Statuses.FirstOrDefault(s => s.StatusName == "Active").ID;
                string existingPlayer = _context.Players.FirstOrDefault(p => p.MemberID == r.Member_ID)?.MemberID;
                if (existingPlayer == null)
                {
                    Player p = new Player
                    {
                        FirstName = r.First_Name,
                        LastName = r.Last_Name,
                        MemberID = r.Member_ID,
                        TeamID = teamID,
                        DivisionID = divisionID,
                        StatusID = statusID
                    };
                    players.Add(p);
                }

            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Players.AddRange(players);
                    _context.SaveChanges();
                    transaction.Commit();
                    if (feedBack == "")
                    {
                        feedBack = "Teams added successfully.";
                    }
                    else if (players.Count > 0)
                    {
                        feedBack = "Teams have been added if they were not already in the database.";
                    }
                    else
                    {
                        feedBack = "Teams already exists in the database. No teams were added.";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    feedBack = $"Error: An error occurred while saving data. {ex.Message}";
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile TheExcel)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Team");
            string feedBack = string.Empty;

            if (TheExcel != null)
            {
                try
                {
                    string mimeType = TheExcel.ContentType;
                    long fileLength = TheExcel.Length;

                    if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
                    {
                        if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
                        {
                            ExcelPackage excel;
                            using (var memoryStream = new MemoryStream())
                            {
                                await TheExcel.CopyToAsync(memoryStream);
                                excel = new ExcelPackage(memoryStream);
                            }

                            var workSheet = excel.Workbook.Worksheets[0];

                            if (workSheet.Cells[1, 8].Text == "Team" &&
                                workSheet.Cells[1, 6].Text == "Division" &&
                                workSheet.Cells[2, 5].Text == DateTime.Now.Year.ToString())
                            {
                                var start = workSheet.Dimension.Start;
                                var end = workSheet.Dimension.End;
                                List<ImportReport> imported = new List<ImportReport>();

                                for (int row = start.Row + 1; row <= end.Row; row++)
                                {
                                    var id = workSheet.Cells[row, 1].ToString();
                                    var firstName = workSheet.Cells[row, 2].Text;
                                    var lastName = workSheet.Cells[row, 3].Text;
                                    var memberID = workSheet.Cells[row, 4].Text;
                                    var club = workSheet.Cells[row, 7].Text;
                                    var season = workSheet.Cells[row, 5].Text;
                                    var teamName = workSheet.Cells[row, 8].Text.Trim(' ', 'U', '1', '3', '5', '8', '9');
                                    var division = workSheet.Cells[row, 6].Text;

                                    ImportReport import = new ImportReport
                                    {
                                        ID = id,
                                        First_Name = firstName,
                                        Last_Name = lastName,
                                        Member_ID = memberID,
                                        Season = season,
                                        Club = club,
                                        Division = division,
                                        Team = teamName
                                    };
                                    imported.Add(import);

                                }
                                ImportTeams(imported, feedBack);
                                Redirect(ViewData["ReturnURL"].ToString());
                                ImportPlayers(imported, feedBack);
                            }
                            else
                            {
                                feedBack = "Error: You may have selected the wrong file to upload.";
                            }
                        }
                        else if (mimeType.Contains("text/csv"))
                        {
                            using (var reader = new StreamReader(TheExcel.OpenReadStream()))
                            using (var csvParser = new TextFieldParser(reader))
                            {
                                csvParser.SetDelimiters(",");

                                // Skip header row
                                if (!csvParser.EndOfData)
                                {
                                    csvParser.ReadLine();
                                }

                                List<ImportReport> imported = new List<ImportReport>();

                                while (!csvParser.EndOfData)
                                {
                                    string[] fields = csvParser.ReadFields();

                                    if (fields.Length >= 3 &&
                                        fields[4] == DateTime.Now.Year.ToString()) // Ensure there are at least 3 fields (Team Name, Coach, Division)
                                    {
                                        var id = fields[0];
                                        var firstName = fields[1];
                                        var lastName = fields[2];
                                        var memberID = fields[3];
                                        var club = fields[6];
                                        var season = fields[4];
                                        var teamName = fields[7].TrimStart(' ', 'U', '1', '3', '5', '8', '9');
                                        var division = fields[5];

                                        ImportReport import = new ImportReport
                                        {
                                            ID = id,
                                            First_Name = firstName,
                                            Last_Name = lastName,
                                            Member_ID = memberID,
                                            Season = season,
                                            Club = club,
                                            Division = division,
                                            Team = teamName
                                        };
                                        imported.Add(import);
                                    }
                                    else
                                    {
                                        feedBack = "Error: CSV file does not have the required columns or is and outdated version.";
                                        break; // Exit the loop as the CSV structure is not as expected
                                    }
                                }
                                ImportTeams(imported, feedBack);
                                Redirect(ViewData["ReturnURL"].ToString());
                                ImportPlayers(imported, feedBack);

                                //using (var transaction = _context.Database.BeginTransaction())
                                //{
                                //    try
                                //    {
                                //        _context.Teams.AddRange(teams);
                                //        _context.SaveChanges();
                                //        transaction.Commit();
                                //        if (feedBack == "")
                                //        {
                                //            feedBack = "Teams added successfully.";
                                //        }
                                //        else if (teams.Count > 0)
                                //        {
                                //            feedBack = "Teams have been added if they were not already in the database.";
                                //        }
                                //        else
                                //        {
                                //            feedBack = "Teams already exists in the database or is and outdated version of the file. No teams were added.";
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        transaction.Rollback();
                                //        feedBack = $"Error: An error occurred while saving data. {ex.Message}";
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            feedBack = "Error: That file is not an Excel spreadsheet.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: File appears to be empty.";
                    }
                }
                catch (Exception ex)
                {
                    feedBack = $"Error: An unexpected error occurred. {ex.Message}" +
                        $"Please try again or contact support.";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded.";
            }

            TempData["Feedback"] = feedBack;
            return Redirect(ViewData["ReturnURL"].ToString());
        }

        // GET: ImportReports
        public async Task<IActionResult> Index()
        {
              return View(await _context.ImportReport.ToListAsync());
        }

        // GET: ImportReports/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.ImportReport == null)
            {
                return NotFound();
            }

            var importReport = await _context.ImportReport
                .FirstOrDefaultAsync(m => m.ID == id);
            if (importReport == null)
            {
                return NotFound();
            }

            return View(importReport);
        }

        // GET: ImportReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImportReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,First_Name,Last_Name,Member_ID,Season,Division,Club,Team")] ImportReport importReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(importReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(importReport);
        }

        // GET: ImportReports/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ImportReport == null)
            {
                return NotFound();
            }

            var importReport = await _context.ImportReport.FindAsync(id);
            if (importReport == null)
            {
                return NotFound();
            }
            return View(importReport);
        }

        // POST: ImportReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,First_Name,Last_Name,Member_ID,Season,Division,Club,Team")] ImportReport importReport)
        {
            if (id != importReport.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportReportExists(importReport.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(importReport);
        }

        // GET: ImportReports/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ImportReport == null)
            {
                return NotFound();
            }

            var importReport = await _context.ImportReport
                .FirstOrDefaultAsync(m => m.ID == id);
            if (importReport == null)
            {
                return NotFound();
            }

            return View(importReport);
        }

        // POST: ImportReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ImportReport == null)
            {
                return Problem("Entity set 'WMBAContext.ImportReport'  is null.");
            }
            var importReport = await _context.ImportReport.FindAsync(id);
            if (importReport != null)
            {
                _context.ImportReport.Remove(importReport);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportReportExists(string id)
        {
          return _context.ImportReport.Any(e => e.ID == id);
        }

    }
}

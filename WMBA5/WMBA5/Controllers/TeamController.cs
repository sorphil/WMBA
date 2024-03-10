using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMBA5.Data;
using WMBA5.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using WMBA5.ViewModels;
using WMBA5.CustomControllers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA5.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Microsoft.VisualBasic.FileIO;

namespace WMBA5.Controllers
{
    public class TeamController : ElephantController
    {
        private readonly WMBAContext _context;

        public TeamController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Team
        public async Task<IActionResult> Index(string SearchString, int? DivisionID, int? page,
            int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Team")
        {
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            string[] sortOptions = new[] { "Team", "Coach", "Division" };
            PopulateDropDownLists();

            var teams = _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Division)
                .AsNoTracking();

            if (DivisionID.HasValue)
            {
                teams = teams.Where(t => t.DivisionID == DivisionID);
                numberFilters++;
            }
            if (!System.String.IsNullOrEmpty(SearchString))
            {
                teams = teams.Where(t => t.TeamName.ToUpper().Contains(SearchString.ToUpper()));
                numberFilters++;
            }
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!System.String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;
                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            if (sortField == "Team")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(t => t.TeamName);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(t => t.TeamName);
                }
            }
            else if (sortField == "Coach")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderByDescending(t => t.Coach);
                }
                else
                {
                    teams = teams
                        .OrderBy(t => t.Coach);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderByDescending(t => t.Division);
                }
                else
                {
                    teams = teams
                        .OrderBy(t => t.Division);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Team/Create
        public IActionResult Create()
        {
            ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            return View();
        }

        // POST: Team/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TeamName,CoachID,DivisionID,LineupID")] Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName", team.CoachID);
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            return View(team);
        }

        // GET: Team/Edit/5
        public async Task<IActionResult> Edit(int? id, string[] selectedOptions)
        {
       
            try
            {
                List<Player> players = new List<Player>();
                if (selectedOptions != null)
                {
                    foreach (var option in selectedOptions)
                    {
                        Player customer = await _context.Players
                                          .Where(c=>c.ID == int.Parse(option))
                                          .FirstOrDefaultAsync();
                    }
                }
                  
            }
            catch (Exception ex)
            {
                string errMsg = ex.GetBaseException().Message;
                ViewData["Message"] = "Error: Could not send update team";
            }
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(f => f.Players)
                .FirstOrDefaultAsync(f => f.ID == id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName", team.CoachID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            PopulateTeamPlayerLists(team);
            return View(team);
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TeamName,CoachID,DivisionID,LineupID")] Team team, string[] selectedOptions)
        {
            try
            {
                if (id != team.ID)
                {
                    return NotFound();
                }
                UpdateTeamPlayerListboxes(selectedOptions, team);

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(team);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TeamExists(team.ID))
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

                ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName", team.CoachID);
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists see your system administrator.");
            }

            
            return View(team);
        }

        // GET: Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'WMBAContext.Teams'  is null.");
            }
            var team = await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (team != null)
                {
                    _context.Teams.Remove(team);
                }

                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());


            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Team. Remember, you cannot delete a Team that has a Players in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(team);
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }
        private SelectList DivisionList(int? selectedId)
        {
            return new SelectList(_context.Divisions
                .OrderBy(d => d.DivisionName), "ID", "DivisionName", selectedId);
        }
        private void PopulateDropDownLists(Team team = null)
        {
            ViewData["DivisionID"] = DivisionList(team?.DivisionID);
        }




        private void PopulateTeamPlayerLists(Team team)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Players;
            var currentOptionsHS = new HashSet<int>(team.Players.Select(b => b.ID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var r in allOptions)
            {
                if (currentOptionsHS.Contains(r.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = r.ID,
                        DisplayText = r.Summary
                    });
                }
                else if (r.TeamID == null)
                {
                    available.Add(new ListOptionVM
                    {
                        ID = r.ID,
                        DisplayText = r.Summary
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }


        private async void UpdateTeamPlayerListboxes(string[] selectedOptions, Team teamToUpdate)
        {
            
            if (selectedOptions == null)
            {
                teamToUpdate.Players = new List<Player>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(teamToUpdate.Players.Select(b => b.ID));
            foreach (var r in _context.Players)
            {
                if (selectedOptionsHS.Contains(r.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(r.ID))//but not currently in the Team - Add it!
                    {
                        teamToUpdate.Players.Add( await _context.Players.FindAsync(r.ID));
                        Player playerToUpdate = await _context.Players.FindAsync(r.ID);
                        playerToUpdate.TeamID = teamToUpdate.ID;
                    }
                }
                else //not selected
                {
                   
                    if (currentOptionsHS.Contains(r.ID))//but is currently in the Function's collection - Remove it!
                    {
                        teamToUpdate.Players.Remove(await _context.Players.FindAsync(r.ID));
                        Player playerToUpdate = await _context.Players.FindAsync(r.ID);
                        playerToUpdate.TeamID = null;
                        _context.Update(playerToUpdate);
                        await _context.SaveChangesAsync();
                    }
               

                }
            }
        }

        //public async Task<IActionResult> InsertFromExcel(IFormFile TheExcel)
        //{
        //    ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Team");
        //    string feedBack = string.Empty;
        //    if (TheExcel != null)
        //    {
        //        string mimeType = TheExcel.ContentType;
        //        long fileLength = TheExcel.Length;
        //        if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
        //        {
        //            if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
        //            {
        //                ExcelPackage excel;
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    await TheExcel.CopyToAsync(memoryStream);
        //                    excel = new ExcelPackage(memoryStream);
        //                }
        //                var workSheet = excel.Workbook.Worksheets[0];
        //                if (workSheet.Cells[1, 1].Text == "Team Name" &&
        //                        workSheet.Cells[1, 2].Text == "Coach")
        //                {
        //                    var start = workSheet.Dimension.Start;
        //                    var end = workSheet.Dimension.End;
        //                    List<Team> teams = new List<Team>();
        //                    for (int row = start.Row + 1; row <= end.Row; row++)
        //                    {
        //                        var coachId = _context.Coaches.FirstOrDefault(c => c.CoachName == workSheet.Cells[row, 2].Text)?.ID;
        //                        var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 3].Text)?.ID;

        //                        if (coachId != null && divisionId != null)
        //                        {
        //                            Team p = new Team
        //                            {
        //                                TeamName = workSheet.Cells[row, 1].Text,
        //                                CoachID = coachId.Value,
        //                                DivisionID = divisionId.Value
        //                            };
        //                            teams.Add(p);
        //                        }
        //                        else
        //                        {
        //                            // Handle the case where Coach or Division was not found
        //                            feedBack = "Error: Coach or Division not found for some records.";
        //                        }
        //                    }

        //                    using (var transaction = _context.Database.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            _context.Teams.AddRange(teams);
        //                            _context.SaveChanges();
        //                            transaction.Commit();
        //                            feedBack = "Teams added successfully.";
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            transaction.Rollback();
        //                            feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    feedBack = "Error: You may have selected the wrong file to upload.";
        //                }
        //            }
        //            else if (mimeType.Contains("text/csv"))
        //            {
        //                using (var reader = new StreamReader(TheExcel.OpenReadStream()))
        //                using (var csvParser = new TextFieldParser(reader))
        //                {
        //                    csvParser.SetDelimiters(",");

        //                    // Skip header row
        //                    if (!csvParser.EndOfData)
        //                    {
        //                        csvParser.ReadLine();
        //                    }

        //                    List<Team> teams = new List<Team>();

        //                    while (!csvParser.EndOfData)
        //                    {
        //                        string[] fields = csvParser.ReadFields();

        //                        if (fields.Length >= 3) // Ensure there are at least 3 fields (Team Name, Coach, Division)
        //                        {
        //                            var coachId = _context.Coaches.FirstOrDefault(c => c.CoachName == fields[1])?.ID;
        //                            var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == fields[2])?.ID;

        //                            if (coachId != null && divisionId != null)
        //                            {
        //                                Team p = new Team
        //                                {
        //                                    TeamName = fields[0],
        //                                    CoachID = coachId.Value,
        //                                    DivisionID = divisionId.Value
        //                                };
        //                                teams.Add(p);
        //                            }
        //                            else
        //                            {
        //                                // Handle the case where Coach or Division was not found
        //                                feedBack = "Error: Coach or Division not found for some records.";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            feedBack = "Error: CSV file does not have the required columns.";
        //                            break; // Exit the loop as the CSV structure is not as expected
        //                        }
        //                    }

        //                    using (var transaction = _context.Database.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            _context.Teams.AddRange(teams);
        //                            _context.SaveChanges();
        //                            transaction.Commit();
        //                            feedBack = "Teams added successfully.";
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            transaction.Rollback();
        //                            feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    TempData["Feedback"] = feedBack;
        //    return Redirect(ViewData["ReturnURL"].ToString());
        //}

        //public async Task<IActionResult> InsertFromExcel(IFormFile TheExcel)
        //{
        //    ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Team");
        //    string feedBack = string.Empty;

        //    if (TheExcel != null)
        //    {
        //        try
        //        {
        //            string mimeType = TheExcel.ContentType;
        //            long fileLength = TheExcel.Length;

        //            if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
        //            {
        //                if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
        //                {
        //                    ExcelPackage excel;
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await TheExcel.CopyToAsync(memoryStream);
        //                        excel = new ExcelPackage(memoryStream);
        //                    }

        //                    var workSheet = excel.Workbook.Worksheets[0];

        //                    if (workSheet.Cells[1, 1].Text == "Team Name" &&
        //                        workSheet.Cells[1, 2].Text == "Coach")
        //                    {
        //                        var start = workSheet.Dimension.Start;
        //                        var end = workSheet.Dimension.End;
        //                        List<Team> teams = new List<Team>();

        //                        for (int row = start.Row + 1; row <= end.Row; row++)
        //                        {
        //                            var teamName = workSheet.Cells[row, 1].Text;
        //                            var coachId = _context.Coaches.FirstOrDefault(c => c.CoachName == workSheet.Cells[row, 2].Text)?.ID;
        //                            var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 3].Text)?.ID;

        //                            if (coachId != null && divisionId != null)
        //                            {
        //                                // Check if the team with the same name already exists
        //                                var existingTeam = _context.Teams.FirstOrDefault(t => t.TeamName == teamName);

        //                                if (existingTeam == null)
        //                                {
        //                                    // Team does not exist, add it to the list
        //                                    Team p = new Team
        //                                    {
        //                                        TeamName = teamName,
        //                                        CoachID = coachId.Value,
        //                                        DivisionID = divisionId.Value
        //                                    };
        //                                    teams.Add(p);
        //                                }
        //                                else
        //                                {
        //                                    // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                    feedBack = $"Error: Team with name '{teamName}' already exists.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // Handle the case where Coach or Division was not found
        //                                feedBack = "Error: Coach or Division not found for some records.";
        //                            }
        //                        }

        //                        using (var transaction = _context.Database.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                _context.Teams.AddRange(teams);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Teams added successfully.";
        //                                }
        //                                else if (teams.Count > 0)
        //                                {
        //                                    feedBack = "Teams have been added if they were not already in the database.";
        //                                } 
        //                                else
        //                                {
        //                                    feedBack = "Teams already exists in the database. No teams were added.";
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                transaction.Rollback();
        //                                feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        feedBack = "Error: You may have selected the wrong file to upload.";
        //                    }
        //                }
        //                else if (mimeType.Contains("text/csv"))
        //                {
        //                    using (var reader = new StreamReader(TheExcel.OpenReadStream()))
        //                    using (var csvParser = new TextFieldParser(reader))
        //                    {
        //                        csvParser.SetDelimiters(",");

        //                        // Skip header row
        //                        if (!csvParser.EndOfData)
        //                        {
        //                            csvParser.ReadLine();
        //                        }

        //                        List<Team> teams = new List<Team>();

        //                        while (!csvParser.EndOfData)
        //                        {
        //                            string[] fields = csvParser.ReadFields();

        //                            if (fields.Length >= 3) // Ensure there are at least 3 fields (Team Name, Coach, Division)
        //                            {
        //                                var teamName = fields[0];
        //                                var coachId = _context.Coaches.FirstOrDefault(c => c.CoachName == fields[1])?.ID;
        //                                var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == fields[2])?.ID;

        //                                if (coachId != null && divisionId != null)
        //                                {
        //                                    // Check if the team with the same name already exists
        //                                    var existingTeam = _context.Teams.FirstOrDefault(t => t.TeamName == teamName);

        //                                    if (existingTeam == null)
        //                                    {
        //                                        // Team does not exist, add it to the list
        //                                        Team p = new Team
        //                                        {
        //                                            TeamName = teamName,
        //                                            CoachID = coachId.Value,
        //                                            DivisionID = divisionId.Value
        //                                        };
        //                                        teams.Add(p);
        //                                    }
        //                                    else
        //                                    {
        //                                        // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                        feedBack = $"Error: Team with name '{teamName}' already exists.";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    // Handle the case where Coach or Division was not found
        //                                    feedBack = "Error: Coach or Division not found for some records.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                feedBack = "Error: CSV file does not have the required columns.";
        //                                break; // Exit the loop as the CSV structure is not as expected
        //                            }
        //                        }

        //                        using (var transaction = _context.Database.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                _context.Teams.AddRange(teams);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Teams added successfully.";
        //                                }
        //                                else if  (teams.Count > 0)
        //                                {
        //                                    feedBack = "Teams have been added if they were not already in the database.";
        //                                }
        //                                else
        //                                {
        //                                    feedBack = "Teams already exists in the database. No teams were added.";
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                transaction.Rollback();
        //                                feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    feedBack = "Error: That file is not an Excel spreadsheet.";
        //                }
        //            }
        //            else
        //            {
        //                feedBack = "Error: File appears to be empty.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            feedBack = $"Error: An unexpected error occurred. {ex.Message}" +
        //                $"Please try again or contact support.";
        //        }
        //    }
        //    else
        //    {
        //        feedBack = "Error: No file uploaded.";
        //    }

        //    TempData["Feedback"] = feedBack;
        //    return Redirect(ViewData["ReturnURL"].ToString());
        //}
        //public async Task<IActionResult> InsertFromExcel(IFormFile TheExcel)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Team");
        //    string feedBack = string.Empty;

        //    if (TheExcel != null)
        //    {
        //        try
        //        {
        //            string mimeType = TheExcel.ContentType;
        //            long fileLength = TheExcel.Length;

        //            if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
        //            {
        //                if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
        //                {
        //                    ExcelPackage excel;
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await TheExcel.CopyToAsync(memoryStream);
        //                        excel = new ExcelPackage(memoryStream);
        //                    }

        //                    var workSheet = excel.Workbook.Worksheets[0];

        //                    if (workSheet.Cells[1, 8].Text == "Team" &&
        //                        workSheet.Cells[1, 6].Text == "Division" &&
        //                        workSheet.Cells[2, 5].Text == DateTime.Now.Year.ToString())
        //                    {
        //                        var start = workSheet.Dimension.Start;
        //                        var end = workSheet.Dimension.End;
        //                        List<Team> teams = new List<Team>();

        //                        for (int row = start.Row + 1; row <= end.Row; row++)
        //                        {
        //                            var teamName = workSheet.Cells[row, 8].Text.Trim(' ', 'U', '1', '3', '5', '8', '9');
        //                            var coachID = _context.Coaches.FirstOrDefault(c => c.CoachName == workSheet.Cells[row, 9].Text)?.ID;
        //                            var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 6].Text)?.ID;

        //                            if (divisionId != null)
        //                            {
        //                                // Check if the team with the same name already exists
        //                                var existingTeam = _context.Teams.FirstOrDefault(t => t.TeamName == teamName);

        //                                if (existingTeam == null)
        //                                {
        //                                    // Team does not exist, add it to the list
        //                                    Team p = new Team
        //                                    {
        //                                        TeamName = teamName,
        //                                        CoachID = coachID.Value,
        //                                        DivisionID = divisionId.Value
        //                                    };
        //                                    teams.Add(p);
        //                                }
        //                                else
        //                                {
        //                                    // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                    feedBack = $"Error: Team with name '{teamName}' already exists.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // Handle the case where Coach or Division was not found
        //                                feedBack = "Error: Coach or Division not found for some records.";
        //                            }
        //                        }

        //                        using (var transaction = _context.Database.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                _context.Teams.AddRange(teams);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Teams added successfully.";
        //                                }
        //                                else if (teams.Count > 0)
        //                                {
        //                                    feedBack = "Teams have been added if they were not already in the database.";
        //                                }
        //                                else
        //                                {
        //                                    feedBack = "Teams already exists in the database. No teams were added.";
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                transaction.Rollback();
        //                                feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        feedBack = "Error: You may have selected the wrong file to upload.";
        //                    }
        //                }
        //                else if (mimeType.Contains("text/csv"))
        //                {
        //                    using (var reader = new StreamReader(TheExcel.OpenReadStream()))
        //                    using (var csvParser = new TextFieldParser(reader))
        //                    {
        //                        csvParser.SetDelimiters(",");

        //                        // Skip header row
        //                        if (!csvParser.EndOfData)
        //                        {
        //                            csvParser.ReadLine();
        //                        }

        //                        List<Team> teams = new List<Team>();

        //                        while (!csvParser.EndOfData)
        //                        {
        //                            string[] fields = csvParser.ReadFields();

        //                            if (fields.Length >= 3 &&
        //                                fields[4] == DateTime.Now.Year.ToString()) // Ensure there are at least 3 fields (Team Name, Coach, Division)
        //                            {
        //                                var teamName = fields[7].TrimStart(' ', 'U', '1', '3', '5', '8', '9');
        //                                var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == fields[5])?.ID;

        //                                if (divisionId != null)
        //                                {
        //                                    // Check if the team with the same name already exists
        //                                    var existingTeam = _context.Teams.FirstOrDefault(t => t.TeamName == teamName);

        //                                    if (existingTeam == null)
        //                                    {
        //                                        // Team does not exist, add it to the list
        //                                        Team t = new Team
        //                                        {
        //                                            TeamName = teamName,
        //                                            DivisionID = divisionId.Value
        //                                        };
        //                                        teams.Add(t);
        //                                    }
        //                                    else
        //                                    {
        //                                        // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                        feedBack = $"Error: Team with name '{teamName}' already exists.";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    // Handle the case where Division was not found
        //                                    feedBack = "Error: Division not found for some records.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                feedBack = "Error: CSV file does not have the required columns or is and outdated version.";
        //                                break; // Exit the loop as the CSV structure is not as expected
        //                            }
        //                        }

        //                        using (var transaction = _context.Database.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                _context.Teams.AddRange(teams);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Teams added successfully.";
        //                                }
        //                                else if (teams.Count > 0)
        //                                {
        //                                    feedBack = "Teams have been added if they were not already in the database.";
        //                                }
        //                                else
        //                                {
        //                                    feedBack = "Teams already exists in the database or is and outdated version of the file. No teams were added.";
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                transaction.Rollback();
        //                                feedBack = $"Error: An error occurred while saving data. {ex.Message}";
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    feedBack = "Error: That file is not an Excel spreadsheet.";
        //                }
        //            }
        //            else
        //            {
        //                feedBack = "Error: File appears to be empty.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            feedBack = $"Error: An unexpected error occurred. {ex.Message}" +
        //                $"Please try again or contact support.";
        //        }
        //    }
        //    else
        //    {
        //        feedBack = "Error: No file uploaded.";
        //    }

        //    TempData["Feedback"] = feedBack;
        //    return Redirect(ViewData["ReturnURL"].ToString());
        //}
    }
}
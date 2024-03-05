using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA5.Data;
using WMBA5.Models;
using WMBA5.CustomControllers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WMBA5.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Numerics;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using Microsoft.VisualBasic.FileIO;

namespace WMBA5.Controllers
{
    public class PlayerController : ElephantController
    {
        private readonly WMBAContext _context;

        public PlayerController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Player
        public async Task<IActionResult> Index(string SearchString, int? TeamID, int? DivisionID, int? StatusID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Players")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player", "Division", "Team" };

            PopulateDropDownLists();

            var players = _context.Players 
                .Include(p=>p.Team)
                .Include(p=>p.Status)
                .Include(p => p.PlayerAtBats)
                .Include(p => p.Stats)
                .Include(p=>p.Division)
                .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
                numberFilters++;
            }
            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                players = players.Where(p => p.DivisionID == DivisionID);
                numberFilters++;
            }
            //Add as many filters as needed
            if (StatusID.HasValue)
            {
                players = players.Where(p => p.StatusID == StatusID);
                numberFilters++;
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.LastName.ToUpper().Contains(SearchString.Trim().ToUpper())
                                  || p.FirstName.ToUpper().Contains(SearchString.Trim().ToUpper()));
                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!string.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Player")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.LastName)
                        .ThenBy(p=>p.FirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
            }
            if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Division);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Division);
                }
            }
            if (sortField == "Team")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Player/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.PlayerAtBats)
                .Include(p => p.Stats)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Player/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();

        }

        // POST: Player/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("ID,MemberID,FirstName,Nickname,LastName,JerseyNumber,StatusID,DivisionID,TeamID")] Player player)
         {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        error.ToString();
                    }
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.MemberID"))
                {
                    ModelState.AddModelError("MemberID", "Unable to save changes. Remember, you cannot have duplicate Member IDs.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists();
            return View(player);
         }

        // GET: Player/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (player == null)
            {
                return NotFound();
            }
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", player.TeamID);
			ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", player.DivisionID);
			ViewData["StatusID"] = new SelectList(_context.Statuses, "ID", "StatusName", player.StatusID);
			return View(player);
        }

        // POST: Player/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id) //, [Bind("ID,MemberID,FirstName,Nickname,LastName,JerseyNumber,StatusID,DivisionID,TeamID")] Player player)
        {
			//Go get the player to update
			var playerToUpdate = await _context.Players
				.Include(p => p.Status)
				.Include(p => p.Team).ThenInclude(p => p.Division)
                .FirstOrDefaultAsync(p => p.ID == id);
            
            if (playerToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
				p => p.MemberID, p => p.FirstName, p => p.LastName, p => p.Nickname,
				p => p.JerseyNumber, p => p.StatusID, p => p.DivisionID, p => p.TeamID))
            {
                try
                {
                    _context.Update(playerToUpdate);
                    await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(playerToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            //ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", playerToUpdate.TeamID);
            PopulateDropDownLists(playerToUpdate);
			ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", playerToUpdate.TeamID);
			ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", playerToUpdate.DivisionID);
			ViewData["StatusID"] = new SelectList(_context.Statuses, "ID", "StatusName", playerToUpdate.StatusID);

			return View(playerToUpdate);
        }

        // GET: Player/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("No Player to Delete.");
            }
            var player = await _context.Players
                .Include (p=>p.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (player != null)
                {
                    _context.Players.Remove(player);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Player. Try again, and if the problem persists see your system administrator");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            
            return View(player);
        }
        private SelectList TeamSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Teams
                .OrderBy(m => m.TeamName), "ID", "TeamName", selectedId);
        }
        private SelectList DivisionSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Divisions
                .OrderBy(m => m.DivisionName), "ID", "DivisionName", selectedId);
        }
        private SelectList StatusSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Statuses
                .OrderBy(m => m.StatusName), "ID", "StatusName", selectedId);
        }
        private void PopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectionList(player?.TeamID);
            ViewData["DivisionID"] = DivisionSelectionList(player?.DivisionID);
            ViewData["StatusID"] = StatusSelectionList(player?.StatusID);
        }
        private bool PlayerExists(int id)
        {
          return _context.Players.Any(e => e.ID == id);
        }
        //public async Task<IActionResult> InsertFromExcel(IFormFile ExcelPlayer)
        //{
        //    ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Player");
        //    string feedBack = string.Empty;
        //    if (ExcelPlayer != null)
        //    {
        //        string mimeType = ExcelPlayer.ContentType;
        //        long fileLength = ExcelPlayer.Length;
        //        if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
        //        {
        //            if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
        //            {
        //                ExcelPackage excel;
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    await ExcelPlayer.CopyToAsync(memoryStream);
        //                    excel = new ExcelPackage(memoryStream);
        //                }
        //                var workSheet = excel.Workbook.Worksheets[0];
        //                if (workSheet.Cells[1, 4].Text == "Team" &&
        //                        workSheet.Cells[1, 6].Text == "Division")
        //                {
        //                    var start = workSheet.Dimension.Start;
        //                    var end = workSheet.Dimension.End;
        //                    List<Player> players = new List<Player>();
        //                    for (int row = start.Row + 1; row <= end.Row; row++)
        //                    {
        //                        Player p = new Player
        //                        {
        //                            FirstName = workSheet.Cells[row, 1].Text,
        //                            LastName = workSheet.Cells[row, 2].Text,
        //                            MemberID = workSheet.Cells[row, 3].Text,
        //                            TeamID = _context.Teams.FirstOrDefault(c => c.TeamName == workSheet.Cells[row, 4].Text).ID,
        //                            StatusID = _context.Statuses.FirstOrDefault(c => c.StatusName == workSheet.Cells[row, 5].Text).ID,
        //                            DivisionID = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 6].Text).ID
        //                        };
        //                        players.Add(p);
        //                        _context.Players.AddRange(players);
        //                        _context.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    feedBack = "Error: That file is not an Excel spreadsheet.";
        //                }
        //            }
        //            else if (mimeType.Contains("text/csv"))
        //            {
        //                var format = new ExcelTextFormat();
        //                format.Delimiter = ',';
        //                bool firstRowIsHeader = true;

        //                using var reader = new System.IO.StreamReader(ExcelPlayer.OpenReadStream());

        //                using ExcelPackage package = new ExcelPackage();
        //                var result = reader.ReadToEnd();
        //                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Imported Report Data");

        //                workSheet.Cells["A1"].LoadFromText(result, format, TableStyles.None, firstRowIsHeader);
        //                if (workSheet.Cells[1, 4].Text == "Team" &&
        //                    workSheet.Cells[1, 6].Text == "Division")
        //                {
        //                    var start = workSheet.Dimension.Start;
        //                    var end = workSheet.Dimension.End;
        //                    List<Player> players = new List<Player>();
        //                    for (int row = start.Row + 1; row <= end.Row; row++)
        //                    {
        //                        Player p = new Player
        //                        {
        //                            FirstName = workSheet.Cells[row, 1].Text,
        //                            LastName = workSheet.Cells[row, 2].Text,
        //                            MemberID = workSheet.Cells[row, 3].Text,
        //                            TeamID = _context.Teams.FirstOrDefault(c => c.TeamName == workSheet.Cells[row, 4].Text).ID,
        //                            StatusID = _context.Statuses.FirstOrDefault(c => c.StatusName == workSheet.Cells[row, 5].Text).ID,
        //                            DivisionID = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 6].Text).ID
        //                        };
        //                        players.Add(p);
        //                    }
        //                    _context.Players.AddRange(players);
        //                    _context.SaveChanges(); ;
        //                }
        //                else
        //                {
        //                    feedBack = "Error: You may have selected the wrong file to upload.";
        //                }
        //            }
        //            else
        //            {
        //                feedBack = "Error: That file is not an Excel spreadsheet.";
        //            }
        //        }
        //        else
        //        {
        //            feedBack = "Error:  file appears to be empty";
        //        }
        //    }
        //    else
        //    {
        //        feedBack = "Error: No file uploaded";
        //    }
        //    TempData["Feedback"] = feedBack;
        //    return Redirect(ViewData["ReturnURL"].ToString());
        //}
        //public async Task<IActionResult> InsertFromExcel(IFormFile ExcelPlayer)
        //{
        //    ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Player");
        //    string feedBack = string.Empty;

        //    if (ExcelPlayer != null)
        //    {
        //        try
        //        {
        //            string mimeType = ExcelPlayer.ContentType;
        //            long fileLength = ExcelPlayer.Length;

        //            if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
        //            {
        //                if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
        //                {
        //                    ExcelPackage excel;
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        await ExcelPlayer.CopyToAsync(memoryStream);
        //                        excel = new ExcelPackage(memoryStream);
        //                    }

        //                    var workSheet = excel.Workbook.Worksheets[0];

        //                    if (workSheet.Cells[1, 4].Text == "Team" &&
        //                        workSheet.Cells[1, 6].Text == "Division")
        //                    {
        //                        var start = workSheet.Dimension.Start;
        //                        var end = workSheet.Dimension.End;
        //                        List<Player> players = new List<Player>();

        //                        for (int row = start.Row + 1; row <= end.Row; row++)
        //                        {
        //                            var firstName = workSheet.Cells[row, 1].Text;
        //                            var lastName = workSheet.Cells[row, 2].Text;
        //                            var memberID = workSheet.Cells[row, 3].Text;
        //                            var teamID = _context.Teams.FirstOrDefault(c => c.TeamName == workSheet.Cells[row, 4].Text)?.ID;
        //                            var statusID = _context.Statuses.FirstOrDefault(c => c.StatusName == workSheet.Cells[row, 5].Text)?.ID;
        //                            var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 6].Text)?.ID;

        //                            if (teamID != null && statusID != null && divisionId != null)
        //                            {
        //                                // Check if the team with the same name already exists
        //                                var existingTeam = _context.Players.FirstOrDefault(t => t.FirstName == firstName);

        //                                if (existingTeam == null)
        //                                {
        //                                    // Team does not exist, add it to the list
        //                                    Player p = new Player
        //                                    {
        //                                        FirstName = firstName,
        //                                        LastName = lastName,
        //                                        MemberID = memberID,
        //                                        TeamID = teamID.Value,
        //                                        StatusID = statusID.Value,
        //                                        DivisionID = divisionId.Value
        //                                    };
        //                                    players.Add(p);
        //                                }
        //                                else
        //                                {
        //                                    // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                    feedBack = $"Error: Player with memeberID '{memberID}' already exists.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // Handle the case where Coach or Division was not found
        //                                feedBack = "Error: Team, Status or Division not found for some records.";
        //                            }
        //                        }

        //                        using (var transaction = _context.Database.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                _context.Players.AddRange(players);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Players added successfully.";
        //                                }
        //                                else if (players.Count > 0)
        //                                {
        //                                    feedBack = "Players have been added if they were not already in the database.";
        //                                }
        //                                else
        //                                {
        //                                    feedBack = "Players already exists in the database. No teams were added.";
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
        //                    using (var reader = new StreamReader(ExcelPlayer.OpenReadStream()))
        //                    using (var csvParser = new TextFieldParser(reader))
        //                    {
        //                        csvParser.SetDelimiters(",");

        //                        // Skip header row
        //                        if (!csvParser.EndOfData)
        //                        {
        //                            csvParser.ReadLine();
        //                        }

        //                        List<Player> players = new List<Player>();

        //                        while (!csvParser.EndOfData)
        //                        {
        //                            string[] fields = csvParser.ReadFields();

        //                            if (fields.Length >= 3) // Ensure there are at least 3 fields (Team Name, Coach, Division)
        //                            {
        //                                var firstName = fields[0];
        //                                var lastName = fields[1];
        //                                var memberID = fields[2];
        //                                var teamID = _context.Teams.FirstOrDefault(c => c.TeamName == fields[3])?.ID;
        //                                var statusID = _context.Statuses.FirstOrDefault(c => c.StatusName == fields[4])?.ID;
        //                                var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == fields[5])?.ID;

        //                                if (statusID != null && teamID != null && divisionId != null)
        //                                {
        //                                    // Check if the team with the same name already exists
        //                                    var existingTeam = _context.Players.FirstOrDefault(t => t.FirstName == firstName);

        //                                    if (existingTeam == null)
        //                                    {
        //                                        // Team does not exist, add it to the list
        //                                        Player p = new Player
        //                                        {
        //                                            FirstName = firstName,
        //                                            LastName = lastName,
        //                                            MemberID = memberID,
        //                                            TeamID = teamID.Value,
        //                                            StatusID = statusID.Value,
        //                                            DivisionID = divisionId.Value
        //                                        };
        //                                        players.Add(p);
        //                                    }
        //                                    else
        //                                    {
        //                                        // Team with the same name already exists, handle accordingly (skip, update, etc.)
        //                                        feedBack = $"Error: Player with memberID '{memberID}' already exists.";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    // Handle the case where Coach or Division was not found
        //                                    feedBack = "Error: Team, Status or Division not found for some records.";
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
        //                                _context.Players.AddRange(players);
        //                                _context.SaveChanges();
        //                                transaction.Commit();
        //                                if (feedBack == "")
        //                                {
        //                                    feedBack = "Players added successfully.";
        //                                }
        //                                else if (players.Count > 0)
        //                                {
        //                                    feedBack = "Players have been added if they were not already in the database.";
        //                                }
        //                                else
        //                                {
        //                                    feedBack = "Players already exists in the database. No teams were added.";
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

        //    TempData["FeedBack"] = feedBack;
        //    return Redirect(ViewData["ReturnURL"].ToString());
        //}
        public async Task<IActionResult> InsertFromExcel(IFormFile ExcelPlayer)
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Player");
            string feedBack = string.Empty;

            if (ExcelPlayer != null)
            {
                try
                {
                    string mimeType = ExcelPlayer.ContentType;
                    long fileLength = ExcelPlayer.Length;

                    if (!(mimeType == "" || fileLength == 0)) // Looks like we have a file!!!
                    {
                        if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("xlsx"))
                        {
                            ExcelPackage excel;
                            using (var memoryStream = new MemoryStream())
                            {
                                await ExcelPlayer.CopyToAsync(memoryStream);
                                excel = new ExcelPackage(memoryStream);
                            }

                            var workSheet = excel.Workbook.Worksheets[0];

                            if (workSheet.Cells[1, 8].Text == "Team" &&
                                workSheet.Cells[1, 6].Text == "Division" &&
                                workSheet.Cells[2, 5].Text == DateTime.Now.Year.ToString())
                            {
                                var start = workSheet.Dimension.Start;
                                var end = workSheet.Dimension.End;
                                List<Player> players = new List<Player>();

                                for (int row = start.Row + 1; row <= end.Row; row++)
                                {
                                    var firstName = workSheet.Cells[row, 2].Text;
                                    var lastName = workSheet.Cells[row, 3].Text;
                                    var memberID = workSheet.Cells[row, 4].Text;
                                    var teamID = _context.Teams.FirstOrDefault(c => c.TeamName == workSheet.Cells[row, 8].Text.Trim(' ', 'U', '1', '3', '5', '8', '9'))?.ID;
                                    var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == workSheet.Cells[row, 6].Text)?.ID;

                                    if (firstName != null && lastName != null)
                                    {
                                        // Check if the team with the same name already exists
                                        var existingTeam = _context.Players.FirstOrDefault(t => t.FirstName == firstName);

                                        if (existingTeam == null)
                                        {
                                            // Team does not exist, add it to the list
                                            Player p = new Player
                                            {
                                                FirstName = firstName,
                                                LastName = lastName,
                                                MemberID = memberID,
                                                TeamID = teamID.Value,
                                                DivisionID = divisionId.Value,
                                            };
                                            players.Add(p);
                                        }
                                        else
                                        {
                                            // Team with the same name already exists, handle accordingly (skip, update, etc.)
                                            feedBack = $"Error: Player with memeberID '{memberID}' already exists.";
                                        }
                                    }
                                    else
                                    {
                                        // Handle the case where Coach or Division was not found
                                        feedBack = "Error: Team, Status or Division not found for some records.";
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
                                            feedBack = "Players added successfully.";
                                        }
                                        else if (players.Count > 0)
                                        {
                                            feedBack = "Players have been added if they were not already in the database.";
                                        }
                                        else
                                        {
                                            feedBack = "Players already exists in the database. No teams were added.";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        feedBack = $"Error: An error occurred while saving data. {ex.Message}";
                                    }
                                }
                            }
                            else
                            {
                                feedBack = "Error: You may have selected the wrong file to upload.";
                            }
                        }
                        else if (mimeType.Contains("text/csv"))
                        {
                            using (var reader = new StreamReader(ExcelPlayer.OpenReadStream()))
                            using (var csvParser = new TextFieldParser(reader))
                            {
                                csvParser.SetDelimiters(",");

                                // Skip header row
                                if (!csvParser.EndOfData)
                                {
                                    csvParser.ReadLine();
                                }

                                List<Player> players = new List<Player>();

                                while (!csvParser.EndOfData)
                                {
                                    string[] fields = csvParser.ReadFields();

                                    if (fields.Length >= 3 &&
                                        fields[4] == DateTime.Now.Year.ToString()) // Ensure there are at least 3 fields (Team Name, Coach, Division)
                                    {
                                        var firstName = fields[1];
                                        var lastName = fields[2];
                                        var memberID = fields[3];
                                        var teamID = _context.Teams.FirstOrDefault(c => c.TeamName == fields[7].TrimStart(' ', 'U', '1', '3', '5', '8', '9'))?.ID;
                                        var divisionId = _context.Divisions.FirstOrDefault(c => c.DivisionName == fields[5])?.ID;
                                        var statusID = _context.Statuses.FirstOrDefault(s => s.StatusName == "Active")?.ID;


                                        if (firstName != null && lastName != null)
                                        {
                                        // Check if the player with the same name already exists
                                        var existingPlayer = _context.Players.FirstOrDefault(t => t.MemberID == memberID);

                                            if (existingPlayer == null)
                                            {
                                            // Player does not exist, add it to the list
                                                Player p = new Player
                                                {
                                                FirstName = firstName,
                                                LastName = lastName,
                                                MemberID = memberID,
                                                TeamID = teamID,
                                                DivisionID = divisionId,
                                                StatusID = statusID
                                                };
                                                players.Add(p);
                                            }
                                            else
                                            {
                                                // Team with the same name already exists, handle accordingly (skip, update, etc.)
                                                feedBack = $"Error: Player with memberID '{memberID}' already exists.";
                                            }
                                        }
                                        else
                                        {
                                            // Handle the case where Coach or Division was not found
                                            feedBack = "Error: Team, Status or Division not found for some records.";
                                        }
                                    }
                                    else
                                    {
                                        feedBack = "Error: CSV file does not have the required columns or is outdated.";
                                        break; // Exit the loop as the CSV structure is not as expected
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
                                            feedBack = "Players added successfully.";
                                        }
                                        else if (players.Count > 0)
                                        {
                                            feedBack = "Players have been added if they were not already in the database.";
                                        }
                                        //else
                                        //{
                                        //    feedBack = "Players already exists in the database or is and outdated version of the file. No players were added.";
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        feedBack = $"Error: An error occurred while saving data. {ex.Message}";
                                    }
                                }
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

            TempData["FeedBack"] = feedBack;
            return Redirect(ViewData["ReturnURL"].ToString());
        }
    }
}

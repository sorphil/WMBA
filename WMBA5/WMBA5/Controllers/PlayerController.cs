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
using Org.BouncyCastle.Utilities.IO;
using Microsoft.AspNetCore.Authorization;

namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor, Trash Pandas 15U Coach, Coach")]
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
                //.Include(p => p.PlayerAtBats)
                .Include(p => p.Stats)
                .Include(p=>p.Division)
                .AsNoTracking();

            //Filter for Trash Pandas 15U Coach
            if (User.IsInRole("Trash Pandas 15U Coach"))
            {
                //1 is the ID for U9
                players = players.Where(t => t.Team.TeamName == "Trash Pandas" && t.Division.DivisionName == "15U");
            }
            //Filter for Trash Pandas 15U Scorekeeper
            if (User.IsInRole("Trash Pandas 15U Scorekeeper"))
            {
                //1 is the ID for U9
                players = players.Where(t => t.Team.TeamName == "Trash Pandas" && t.Division.DivisionName == "15U");
            }
            //Filter for Rookie Convenor
            if (User.IsInRole("Rookie Convenor"))
            {
                //1 is the ID for U9
                players = players.Where(t => t.DivisionID == 1);
            }
            //Filter for Intermeditate Convenor
            if (User.IsInRole("Intermediate Convenor"))
            {
                //2 is the ID for U11 and 3 for U13
                players = players.Where(t => t.DivisionID == 2 || t.DivisionID == 3);
            }
            //Filter for senior Convenor
            if (User.IsInRole("Senior Convenor"))
            {
                //4 is the ID for U15
                players = players.Where(t => t.DivisionID >= 4);
            }
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
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName");
            return View();

        }

        // POST: Player/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
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

            var teamsinDiv = _context.Teams.Where(t => t.DivisionID == player.DivisionID).Select(t => new SelectListItem { Value = t.ID.ToString(), Text = t.TeamName });

            ViewData["TeamID"] = new SelectList(teamsinDiv, "Value", "Text", player.TeamID);
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
            var teamsinDiv = _context.Teams.Where(t => t.DivisionID == playerToUpdate.DivisionID).Select(t => new SelectListItem { Value = t.ID.ToString(), Text = t.TeamName });
            //ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", playerToUpdate.TeamID);
            PopulateDropDownLists(playerToUpdate);
            ViewData["TeamID"] = new SelectList(teamsinDiv, "Value", "Text", playerToUpdate.TeamID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", playerToUpdate.DivisionID);
			ViewData["StatusID"] = new SelectList(_context.Statuses, "ID", "StatusName", playerToUpdate.StatusID);

			return View(playerToUpdate);
        }

        // GET: Player/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        private SelectList TeamSelectionList(int? selectedTeamId, int? divisionID)
        {
            if (divisionID.HasValue)
            {
                var teams = _context.Teams
                    .Where(t => t.DivisionID == divisionID)
                    .OrderBy(t => t.TeamName)
                    .ToList();

                if (teams.Any())
                {
                    return new SelectList(teams, "ID", "TeamName", selectedTeamId);
                }
                else
                {
                    return new SelectList(new List<Team> { new Team { ID = 0, TeamName = "Select a team" } }, "ID", "TeamName", selectedTeamId);
                }

            }
            else
            {
                return new SelectList(new List<Team>(), "ID", "TeamName", null);
            }
        }
        private SelectList DivisionSelectionList(int? selectedDivisionId)
        {
            var divisions = _context.Divisions
                .OrderBy(d => d.DivisionName)
                .ToList();

            return new SelectList(divisions, "ID", "DivisionName", selectedDivisionId);
        }
        private SelectList StatusSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Statuses
                .OrderBy(m => m.StatusName), "ID", "StatusName", selectedId);
        }
        private void PopulateDropDownLists(Player player = null)
        {
            ViewData["DivisionID"] = DivisionSelectionList(player?.DivisionID);
            ViewData["StatusID"] = StatusSelectionList(player?.StatusID);

            // Only populate TeamID if DivisionID is selected
            if (player?.DivisionID != null)
            {
                // Filter teams by the selected division
                var teams = _context.Teams
                    .Where(t => t.DivisionID == player.DivisionID)
                    .OrderBy(t => t.TeamName)
                    .ToList();
                ViewData["TeamID"] = new SelectList(teams, "ID", "TeamName", player.TeamID);
            }
            else
            {
                // If no division is selected, show an empty team list
                ViewData["TeamID"] = new SelectList(new List<Team>(), "ID", "TeamName", null);
            }
        }

        // GET: Player/GetTeamsByDivision
        public IActionResult GetTeamsByDivision(int divisionId)
        {
            var teams = _context.Teams
                .Where(t => t.DivisionID == divisionId)
                .OrderBy(t => t.TeamName)
                .Select(t => new { value = t.ID, text = t.TeamName })
                .ToList();

            return Json(teams);
        }
        private bool PlayerExists(int id)
        {
          return _context.Players.Any(e => e.ID == id);
        }
    }
}

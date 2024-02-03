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
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    }
}
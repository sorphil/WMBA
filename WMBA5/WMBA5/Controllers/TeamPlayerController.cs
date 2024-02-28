using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;
using WMBA5.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA5.Controllers
{
    public class TeamPlayerController : ElephantController
    {
        private readonly WMBAContext _context;

        public TeamPlayerController(WMBAContext context)
        {
            _context = context;
        }

        // GET: TeamPlayer
        public async Task<IActionResult> Index(int? TeamID, string SearchString,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Players")
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Team");

            if (!TeamID.HasValue)
            {
                return Redirect(ViewData["returnURL"].ToString());
            }
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player" };
            
      
            PopulateDropDownLists();

            //var players = _context.Players
            //    .Include(t => t.Team)
            //    .Include(t => t.PlayerAtBats)
            //    .Include(t => t.Stats)
            //    .Where(t => t.TeamID == TeamID)
            //    .AsNoTracking();
            var players = from p in _context.Players
                            .Include(t => t.Team)
                          where p.TeamID == TeamID.GetValueOrDefault()
                          select p;


            //Add as many filters as needed
            if (!System.String.IsNullOrEmpty(SearchString))
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
            if (!System.String.IsNullOrEmpty(actionButton)) //Form Submitted!
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
            if (sortField == "Player")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.FirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.FirstName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            Team team = await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Division)
                .Where(t => t.ID == TeamID.GetValueOrDefault())
                .AsNoTracking()
                .FirstOrDefaultAsync();


            ViewBag.Team = team;


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
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Player/Create
        public IActionResult Add()
        {
            Player player = new Player();
            PopulateDropDownLists();
            return View(player);

        }

        // POST: Player/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,MemberID,FirstName,Nickname,LastName,JerseyNumber,StatusID,DivisionID,TeamID")] Player player, string TeamName)
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
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
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
        public async Task<IActionResult> Update(int id)
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
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
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
        public async Task<IActionResult> Remove(int? id)
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
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WMBAContext.Players'  is null.");
            }
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    }
}

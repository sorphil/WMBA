using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;
using WMBA5.Utilities;

namespace WMBA5.Controllers
{
    public class PlayerStatController : ElephantController
    {
        private readonly WMBAContext _context;

        public PlayerStatController(WMBAContext context)
        {
            _context = context;
        }

        // GET: PlayerStat
        public async Task<IActionResult> Index(int? PlayerID, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "PlayerAppearance")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Player");
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player Appearances", "Hits", "Runs Scored", "Strike Outs", "Walks", "RBI" };

            var playerStats = _context.PlayerStats
            .Where(ps => ps.PlayerID == PlayerID)
            .AsNoTracking();


            if (!PlayerID.HasValue)
            {
                //Go back to the proper return URL for the Player controller
                return Redirect(ViewData["returnURL"].ToString());
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
            if (sortField == "Player Appearances")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Stat.PlayerAppearance);
                        //.ThenBy(p => p.PlayerAppearance);
                }
                else
                {
                    playerStats = playerStats
                            .OrderByDescending(p => p.Stat.PlayerAppearance);
                       
                }
            }
            
            else if (sortField == "Hits")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Stat.Hits);
                }
                else
                {
                    playerStats = playerStats
                            .OrderByDescending(p => p.Stat.Hits);
                }
            }
            else if (sortField == "Runs Scored")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                            .OrderBy(p => p.Stat.RunsScored);
                }
                else
                {
                    playerStats = playerStats
                               .OrderByDescending(p => p.Stat.RunsScored);

                }
            }
            else if (sortField == "Walks")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                            .OrderBy(p => p.Stat.Walks);
                }
                else
                {
                    playerStats = playerStats
                               .OrderByDescending(p => p.Stat.Walks);
                }
            }
            else if (sortField == "Strike Outs")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                            .OrderBy(p => p.Stat.StrikeOuts)
                            .ThenBy(p => p.Stat.StrikeOuts);
                }
                else
                {
                    playerStats = playerStats
                               .OrderByDescending(p => p.Stat.StrikeOuts);
                }
            }
            else if (sortField == "RBI")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                            .OrderBy(p => p.Stat.RBI);
                }
                else
                {
                    playerStats = playerStats
                               .OrderByDescending(p => p.Stat.RBI);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            Player player = await _context.Players
            .Include(p => p.Team)
            .Include( p => p.Status)
            .Include(p => p.PlayerAtBats)
            .Include(p => p.PlayerStats)
            .ThenInclude(ps=>ps.Stat)
            .Include(p => p.Division)
            .Where(t => t.ID == PlayerID.GetValueOrDefault())
            .AsNoTracking()
            .FirstOrDefaultAsync();

            ViewBag.Player = player;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<PlayerStat>.CreateAsync(playerStats.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: PlayerStat/Create
        public IActionResult Create()
        {
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName");
            return View();
        }

        // POST: PlayerStat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MemberID,FirstName,Nickname,LastName,JerseyNumber,TeamID")] Player player)
        {
            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", player.TeamID);
            return View(player);
        }

        // GET: PlayerStat/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(player);
        }

        // POST: PlayerStat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MemberID,FirstName,Nickname,LastName,JerseyNumber,TeamID")] Player player)
        {
            if (id != player.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.ID))
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
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName", player.TeamID);
            return View(player);
        }

        // GET: PlayerStat/Delete/5
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

        // POST: PlayerStat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}


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
using WMBA5.ViewModels;

namespace WMBA5.Controllers
{
    public class PlayerStatScoreController : ElephantController
    {
        private readonly WMBAContext _context;

        public PlayerStatScoreController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Stat
        public async Task<IActionResult> Index(int? PlayerID, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "PlayerAppearance")
        {

            Player player = await _context.Players
             .Include(p => p.Team)
             .Include(p => p.Status)

             .Include(p => p.Stats)
             .Include(p => p.Division)
             .Where(t => t.ID == PlayerID)
             .AsNoTracking()
             .FirstOrDefaultAsync();

            var games = _context.Games
            .Include(g=>g.AwayTeam)
            .Include(g=>g.HomeTeam)
            .Include(g=>g.Location)
            .Where(g => (g.AwayTeamID == player.TeamID) || (g.HomeTeamID == player.TeamID))
            .AsNoTracking();

            var stats = _context.Stats
                .Where(s=>s.PlayerID==PlayerID)
                .AsNoTracking()
                .FirstOrDefault();


            if (!PlayerID.HasValue)
            {
                //Go back to the proper return URL for the Player controller
                return Redirect(ViewData["returnURL"].ToString());
            }


            ViewBag.Player = player;
            ViewBag.Stats = stats;

            //Handle Paging
            //int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            //ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            //var pagedData = await PaginatedList<GameStatsVM>.CreateAsync(Stats, page ?? 1, pageSize);

            return View(games);
        }
        public async Task<IActionResult> Details(int? PlayerID, int? GameID)
        {

            Player player = await _context.Players
             .Include(p => p.Team)
             .Include(p => p.Status)

             .Include(p => p.Stats)
             .Include(p => p.Division)
             .Where(t => t.ID == PlayerID.GetValueOrDefault())
             .AsNoTracking()
             .FirstOrDefaultAsync();


            var stats = _context.Stats
             .Where(s => s.PlayerID == PlayerID)
             .AsNoTracking()
             .FirstOrDefault();

            var scores = _context.Scores
              .Include(s => s.Inning)
              .Include(s => s.Game)
              .Include(s => s.Player)
              .Where(ps => ps.PlayerID == PlayerID)
              .AsNoTracking();

            ViewData["GameSummary"] = _context.Games.Where(g => g.ID == GameID)
                .Include(g=>g.HomeTeam)
                .Include(g=>g.AwayTeam)
                .FirstOrDefault().Summary;

            ViewData["GameID"] = _context.Games.Where(g => g.ID == GameID)
             .Include(g => g.HomeTeam)
             .Include(g => g.AwayTeam)
             .FirstOrDefault().ID;



            ViewBag.Stats = stats;
            ViewBag.Scores = scores;
            ViewBag.Player = player;
            if (player == null)
            {
                return NotFound();
            }

            return View(stats);
        }

        // GET: Stat/Create
        public IActionResult Create()
        {
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TeamName");
            return View();
        }

        // POST: Stat/Create
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

        // GET: Stat/Edit/5
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

        // POST: Stat/Edit/5
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

        // GET: Stat/Delete/5
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

        // POST: Stat/Delete/5
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA5.Data;
using WMBA5.Models;

namespace WMBA5.Controllers
{
    public class StatController : Controller
    {
        private readonly WMBAContext _context;

        public StatController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Stat
        public async Task<IActionResult> Index()
        {
              return View(await _context.Stats.ToListAsync());
        }

        // GET: Stat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Stats == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats
                .FirstOrDefaultAsync(m => m.ID == id);
            if (stat == null)
            {
                return NotFound();
            }

            return View(stat);
        }

        // GET: Stat/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,GamesPlayed,PlayerAppearance,Hits,RunsScored,StrikeOuts,Walks,RBI")] Stat stat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stat);
        }

        // GET: Stat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Stats == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats.FindAsync(id);
            if (stat == null)
            {
                return NotFound();
            }
            return View(stat);
        }

        // POST: Stat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,GamesPlayed,PlayerAppearance,Hits,RunsScored,StrikeOuts,Walks,RBI")] Stat stat)
        {
            if (id != stat.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatExists(stat.ID))
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
            return View(stat);
        }

        // GET: Stat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Stats == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats
                .FirstOrDefaultAsync(m => m.ID == id);
            if (stat == null)
            {
                return NotFound();
            }

            return View(stat);
        }

        // POST: Stat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Stats == null)
            {
                return Problem("Entity set 'WMBAContext.Stats'  is null.");
            }
            var stat = await _context.Stats.FindAsync(id);
            if (stat != null)
            {
                _context.Stats.Remove(stat);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatExists(int id)
        {
          return _context.Stats.Any(e => e.ID == id);
        }
    }
}

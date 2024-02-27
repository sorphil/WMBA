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
    public class ScoreController : Controller
    {
        private readonly WMBAContext _context;

        public ScoreController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Score
        public async Task<IActionResult> Index()
        {
              return View(await _context.Score.ToListAsync());
        }

        // GET: Score/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Score == null)
            {
                return NotFound();
            }

            var score = await _context.Score
                .FirstOrDefaultAsync(m => m.ID == id);
            if (score == null)
            {
                return NotFound();
            }

            return View(score);
        }

        // GET: Score/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Score/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Balls,FoulBalls,Strikes,Out,Runs,Hits")] Score score)
        {
            if (ModelState.IsValid)
            {
                _context.Add(score);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(score);
        }

        // GET: Score/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Score == null)
            {
                return NotFound();
            }

            var score = await _context.Score.FindAsync(id);
            if (score == null)
            {
                return NotFound();
            }
            return View(score);
        }

        // POST: Score/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Balls,FoulBalls,Strikes,Out,Runs,Hits")] Score score)
        {
            if (id != score.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(score);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoreExists(score.ID))
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
            return View(score);
        }

        // GET: Score/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Score == null)
            {
                return NotFound();
            }

            var score = await _context.Score
                .FirstOrDefaultAsync(m => m.ID == id);
            if (score == null)
            {
                return NotFound();
            }

            return View(score);
        }

        // POST: Score/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Score == null)
            {
                return Problem("Entity set 'WMBAContext.Score'  is null.");
            }
            var score = await _context.Score.FindAsync(id);
            if (score != null)
            {
                _context.Score.Remove(score);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoreExists(int id)
        {
          return _context.Score.Any(e => e.ID == id);
        }
    }
}

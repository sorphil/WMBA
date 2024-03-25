using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;

namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
    public class CoachController : ElephantController
    {
        private readonly WMBAContext _context;

        public CoachController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Coach
        public async Task<IActionResult> Index()
        {
              return View(await _context.Coaches.ToListAsync());
        }

        // GET: Coach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // GET: Coach/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CoachName")] Coach coach)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(coach);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new {coach.ID});
                }
            }
            catch (RetryLimitExceededException/* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(coach);
        }

        // GET: Coach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .FirstOrDefaultAsync(d => d.ID == id);

            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var coachToUpdate = await _context.Coaches
                .FirstOrDefaultAsync(d => d.ID == id);

            if (coachToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Coach>(coachToUpdate,"", d=>d.CoachName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { coachToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachExists(coachToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(coachToUpdate);
        }

        // GET: Coach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Coaches == null)
            {
                return Problem("No Coach to Delete.");
            }
            var coach = await _context.Coaches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (coach != null)
                {
                    _context.Coaches.Remove(coach);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Coach. Remember, you cannot delete a Coach who is managing a team.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(coach);
        }

        private bool CoachExists(int id)
        {
          return _context.Coaches.Any(e => e.ID == id);
        }
    }
}

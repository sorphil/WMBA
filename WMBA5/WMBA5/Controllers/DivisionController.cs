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

namespace WMBA5.Controllers
{
    public class DivisionController : ElephantController
    {
        private readonly WMBAContext _context;

        public DivisionController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Division
        public async Task<IActionResult> Index()
        {
            var wMBAContext = _context.Divisions.Include(d => d.Club);
            return View(await wMBAContext.ToListAsync());
        }

        // GET: Division/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // GET: Division/Create
        public IActionResult Create()
        {
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName");
            Division division = new Division();
            return View(division);
        }

        // POST: Division/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DivisionName,ClubID")] Division division)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(division);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new {division.ID});
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // GET: Division/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // POST: Division/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var divisionToUpdate = await _context.Divisions
                .FirstOrDefaultAsync(a => a.ID == id);

            if (divisionToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Division>(divisionToUpdate,"", a => a.DivisionName, a => a.ClubID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new {divisionToUpdate.ID});
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(divisionToUpdate.ID))
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
                return RedirectToAction("Details", new { divisionToUpdate.ID });
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", divisionToUpdate.ClubID);
            return View(divisionToUpdate);
        }

        // GET: Division/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Division/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Divisions == null)
            {
                return Problem("No Division to Delete.");
            }
            var division = await _context.Divisions
                .Include(d => d.Club).FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (division != null)
                {
                    _context.Divisions.Remove(division);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Division. You cannot delete a Division if it has team or players in it.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(division);
        }

        private bool DivisionExists(int id)
        {
          return _context.Divisions.Any(e => e.ID == id);
        }
    }
}

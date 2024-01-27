using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMBA5.Data;
using WMBA5.Models;

namespace WMBA5.Controllers
{
    public class TeamController : Controller
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
            string[] sortOptions = new[] { "Team", "Coach" };
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
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //Handle Paging
            //int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            //ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            //var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);
            return View(await teams.ToListAsync());
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
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName", team.CoachID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View(team);
        }

        // GET: Team/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "CoachName", team.CoachID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View(team);
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TeamName,CoachID,DivisionID,LineupID")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

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
    }
}
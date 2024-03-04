using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System.Linq;

using System.Numerics;

using System.Linq.Expressions;

using System.Threading.Tasks;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;

namespace WMBA5.Controllers
{
    public class GameController : ElephantController
    {
        private readonly WMBAContext _context;

        public GameController(WMBAContext context)
        {
            _context = context;
        }

        // GET: Game with filtering, searching, and sorting
        public async Task<IActionResult> Index(int? DivisionID, int? OutcomeID, int? LocationID, int? HomeTeamID, int? AwayTeamID,
            string SearchString, string actionButton, string sortDirection = "asc", string sortField = "Location")
        {
            PopulateDropDownLists();

            ViewData["CurrentFilter"] = SearchString;

            //var gamesQuery = _context.Games.Include(g => g.Division).AsQueryable();

            var gamesQuery = _context.Games.Include(g => g.Division)
                                .Include(g => g.Outcome)
                                .Include(g => g.Location)
                                .Include(g => g.AwayTeam)
                                .Include(g => g.HomeTeam)
                                .AsNoTracking();

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player" };


            // Filtering
            if (DivisionID.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.DivisionID == DivisionID);
                numberFilters++;
            }
            if (OutcomeID.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.OutcomeID == OutcomeID);
                numberFilters++;
            }
            if (LocationID.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.LocationID == LocationID);
                numberFilters++;
            }
            if (HomeTeamID.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.HomeTeamID == HomeTeamID || g.HomeTeam.ID == HomeTeamID);
                numberFilters++;
            }
            if (AwayTeamID.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.AwayTeamID == AwayTeamID || g.AwayTeam.ID == AwayTeamID);
                numberFilters++;
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                //gamesQuery = gamesQuery.Where(g => g.Oponent.Contains(SearchString));
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

            // Sorting
            if (!string.IsNullOrEmpty(actionButton))
            {
                if (actionButton == sortField) // Toggle sort direction on same field
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                else
                {
                    sortDirection = "asc"; // Default to ascending on new sort field
                }
                sortField = actionButton; // Set the sort field based on the clicked button
            }

            switch (sortField)
            {
                case "Location":
                    gamesQuery = sortDirection == "asc" ? gamesQuery.OrderBy(g => g.Location) : gamesQuery.OrderByDescending(g => g.Location);
                    break;
                case "Oponent":
                    //gamesQuery = sortDirection == "asc" ? gamesQuery.OrderBy(g => g.Oponent) : gamesQuery.OrderByDescending(g => g.Oponent);
                    break;
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            PopulateDropDownLists();

            return View(await gamesQuery.ToListAsync());
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g=>g.Outcome)
                .Include(g=>g.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);


            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Game/Create
        public IActionResult Create()
        {
            // Keeping your original SelectList assignments for other fields
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["OutcomeID"] = new SelectList(_context.Outcomes, "ID", "OutcomeString");

            // Constructing the team list with divID as a string before the team names
            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            // Using the constructed teamList for both AwayTeamID and HomeTeamID dropdowns
            ViewData["Teams"] = new SelectList(teamList, "Value", "Text");

            // Proceeding with your Create action logic...
            return View();
        }


        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StartTime,HomeTeamID,AwayTeamID,LocationID,OutcomeID, DivisionID")] Game game, int HomeTeamID, int AwayTeamID)
        {
            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            try
            {
                var homeTeam = await _context.Teams.Include(t => t.Division).FirstOrDefaultAsync(t => t.ID == HomeTeamID);
                var awayTeam = await _context.Teams.Include(t => t.Division).FirstOrDefaultAsync(t => t.ID == AwayTeamID);

                if (homeTeam.DivisionID != game.DivisionID || awayTeam.DivisionID != game.DivisionID)
                {
                    ModelState.AddModelError("", "The selected teams must belong to the selected game's division.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                PopulateDropDownLists(game);

                if (HomeTeamID == 0 || AwayTeamID == 0)
                {
                    ModelState.AddModelError("", "Home Team and Away Team are required.");
                    PopulateDropDownLists(game);
                    ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
                    ViewData["Teams"] = new SelectList(teamList, "Value", "Text");
                    return View(game);
                }

                if (homeTeam.DivisionID != awayTeam.DivisionID)
                {
                    ModelState.AddModelError("", "Home Team and Away Team must belong to the same division.");
                    PopulateDropDownLists(game);
                    ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
                    ViewData["Teams"] = new SelectList(teamList, "Value", "Text");
                    return View(game);
                }

                if (ModelState.IsValid)
                {
                    //Load all of the Teams with Players into the game object first
                    game.HomeTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.HomeTeamID);
                    game.AwayTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.AwayTeamID);

                    //Set the initial lineups with all team members
                    FillLineupsWithTeams(game);

                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = game.ID });
                }
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
                ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TeamName");
                ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TeamName");
                ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
                ViewData["OutcomeID"] = new SelectList(_context.Outcomes, "ID", "OutcomeString");

            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

            }

            return View(game);
        }

        private void FillLineupsWithTeams(Game game)
        {
            //Add the players from the teams to each one
            foreach (Player player in game.HomeTeam.Players)
            {
                game.GamePlayers.Add(new GamePlayer()
                {
                    PlayerID = player.ID,
                    GameID = game.ID,
                    TeamLineup = TeamLineup.Home
                });
            }

            foreach (Player player in game.AwayTeam.Players)
            {
                game.GamePlayers.Add(new GamePlayer()
                {
                    PlayerID = player.ID,
                    GameID = game.ID,
                    TeamLineup = TeamLineup.Away
                });
            }
        }
        // GET: Game/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g => g.Location)
                .Include(g => g.Outcome)
                .FirstOrDefaultAsync(g => g.ID == id);

            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            if (game == null)
            {
                return NotFound();
            }

            PopulateDropDownLists(game); // Use the existing method to populate drop-down lists
            return View(game);
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StartTime,HomeTeamID,AwayTeamID,LocationID,OutcomeID, DivisionID")] Game game, int HomeTeamID, int AwayTeamID)
        {
            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            if (id != game.ID)
            {
                return NotFound();
            }

            var homeTeam = await _context.Teams.Include(t => t.Division).FirstOrDefaultAsync(t => t.ID == HomeTeamID);
            var awayTeam = await _context.Teams.Include(t => t.Division).FirstOrDefaultAsync(t => t.ID == AwayTeamID);

            if (HomeTeamID == 0 || AwayTeamID == 0)
            {
                ModelState.AddModelError("", "Home Team and Away Team are required.");
                PopulateDropDownLists(game);
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
                ViewData["Teams"] = new SelectList(teamList, "Value", "Text");
                return View(game);
            }

            if (homeTeam.DivisionID != game.DivisionID || awayTeam.DivisionID != game.DivisionID)
            {
                ModelState.AddModelError("", "The selected teams must belong to the selected game's division.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownLists(game);

            if (homeTeam.DivisionID != awayTeam.DivisionID)
            {
                ModelState.AddModelError("", "Home Team and Away Team must belong to the same division.");
                PopulateDropDownLists(game);
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
                ViewData["Teams"] = new SelectList(teamList, "Value", "Text");
                return View(game);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            PopulateDropDownLists(game); // Repopulate dropdown lists if validation fails
            return View(game);
        }

        //Creating the action to record the in-game Stats for futher creation of the view
        public async Task<IActionResult> InGameStatsRecord(Game game, int? id)
        {
            var gameStats = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g => g.Outcome)
                .Include(g => g.Location)
                .Include(g => g.Innings).ThenInclude(g=> g.Scores)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ModelState.IsValid)
            {
                

                
            }   

            return View(gameStats);
        }

        // GET: Game/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Division)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g=>g.Outcome)
                .Include(g=>g.Location)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private SelectList LocationSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Locations
                .OrderBy(m => m.LocationName), "ID", "LocationName", selectedId);
        }
        private SelectList DivisionSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Divisions
                .OrderBy(m => m.DivisionName), "ID", "DivisionName", selectedId);
        }
        private SelectList OutcomeSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Outcomes
                .OrderBy(m => m.OutcomeString), "ID", "OutcomeString", selectedId);
        }
        private SelectList TeamSelectionList(int? selectedId)
        {
            return new SelectList(_context
                .Teams
                .OrderBy(m => m.TeamName), "ID", "TeamName", selectedId);
        }
        private void PopulateDropDownLists(Game? game = null)
        {

            ViewData["OutcomeID"] = OutcomeSelectionList(game?.OutcomeID);
            ViewData["DivisionID"] = DivisionSelectionList(game?.DivisionID);
            ViewData["LocationID"] = LocationSelectionList(game?.LocationID);
            ViewData["HomeTeamID"] = TeamSelectionList(game?.HomeTeamID);
            ViewData["AwayTeamID"] = TeamSelectionList(game?.AwayTeamID);

            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["OutcomeID"] = new SelectList(_context.Outcomes, "ID", "OutcomeString");

            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            ViewData["Teams"] = new SelectList(teamList, "Value", "Text");

        }

        [HttpGet]
        public JsonResult GetLocations(int? id)
        {
            return Json(LocationSelectionList(id));
        }
        private bool GameExists(int id) => _context.Games.Any(e => e.ID == id);
    }
}

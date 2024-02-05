using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System.Linq;
using System.Numerics;
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
        public async Task<IActionResult> Index(int? DivisionID, string SearchString, string actionButton, string sortDirection = "asc", string sortField = "Location")
        {
            PopulateDropDownLists();

            ViewData["CurrentFilter"] = SearchString;

            //var gamesQuery = _context.Games.Include(g => g.Division).AsQueryable();

            var gamesQuery = _context.Games.Include(g => g.Division)
                                .Include(g => g.TeamGame)
                                    .ThenInclude(tg => tg.HomeTeam)
                                .Include(g => g.TeamGame)
                                    .ThenInclude(tg => tg.AwayTeam)
                                .AsTracking();

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
                .Include(g => g.Division)
                .Include(g => g.TeamGame)
                .ThenInclude(tg => tg.HomeTeam)
                .Include(g => g.TeamGame)
                .ThenInclude(tg => tg.AwayTeam)
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
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["Teams"] = new SelectList(_context.Teams, "ID", "TeamName");

            // Create a new Game instance with an associated TeamGame
            var newGame = new Game
            {
                TeamGame = new TeamGame()
            };

            return View(newGame);
        }

        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StartTime,Location,Oponent,PlayingAt,Outcome,DivisionID,LineupID")] Game game, int selectedHomeTeam, int selectedAwayTeam)
        {
            if (ModelState.IsValid)
            {
                // Create a new TeamGame and associate it with the game
                var teamGame = new TeamGame { HomeTeamID = selectedHomeTeam, AwayTeamID = selectedAwayTeam, GameID = game.ID };
                game.TeamGame = teamGame;

                // Add the game and teamGame to the context
                _context.Add(game);
                _context.Add(teamGame);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownList(game);

            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            ViewData["Teams"] = new SelectList(_context.Teams, "ID", "TeamName");
            return View(game);
        }

        // GET: Game/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(d => d.TeamGame).ThenInclude(d => d.HomeTeam)
               .Include(d => d.TeamGame).ThenInclude(d => d.AwayTeam)
               .Include(d => d.Division)
               .FirstOrDefaultAsync(d => d.ID == id);

            if (game == null)
            {
                return NotFound();
            }
            PopulateDropDownList(game);

            ViewData["Teams"] = new SelectList(_context.Teams, "ID", "TeamName");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            return View(game);
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StartTime,Location,Oponent,PlayingAt,Outcome,DivisionID,LineupID")] Game game, int selectedHomeTeam, int selectedAwayTeam)
        {
            if (id != game.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (selectedHomeTeam == 0 || selectedAwayTeam == 0)
                    {
                        ModelState.AddModelError("", "Home Team and Away Team are required.");
                        PopulateDropDownList(game);
                        ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionSummary", game.DivisionID);
                        ViewData["Teams"] = new SelectList(_context.Teams, "ID", "TeamName");
                        return View(game);
                    }
                    // Find the existing game in the database
                    var existingGame = await _context.Games
                        .Include(g => g.TeamGame)
                        .FirstOrDefaultAsync(m => m.ID == id);

                    if (existingGame == null)
                    {
                        return NotFound();
                    }

                    // Update the game properties
                    existingGame.DivisionID = game.DivisionID;

                    // Update the associated TeamGame
                    if (existingGame.TeamGame == null)
                    {
                        existingGame.TeamGame = new TeamGame();
                    }

                    existingGame.TeamGame.HomeTeamID = selectedHomeTeam;
                    existingGame.TeamGame.AwayTeamID = selectedAwayTeam;

                    // Save changes
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["Teams"] = new SelectList(_context.Teams, "ID", "TeamName");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            return View(game);
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
        
        

        //Selecting the division for game
        private SelectList DivisionSelectionList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivisionName", selectedId);
        }

        //Selecting Teams that are part of the division we select
		//private SelectList TeamSelectionList(int? selectedId, int DivisionID)
		//{
		//	var query = from t in _context.Teams
		//				where t.DivisionID == DivisionID
		//				select t;
		//	return new SelectList(query.OrderBy(p => p.TeamName), "ID", "TeamSummary", selectedId);

			
		//}
		private void PopulateDropDownLists(Game game = null)
		{
			ViewData["DivisionID"] = DivisionSelectionList(game?.DivisionID);
		}
		//private void PopulateDropDownList(Game game)
		//{

		//	if ((game?.DivisionID).HasValue)
		//	{   //Careful: CityID might have a value but the City object could be missing
		//		if (game.Division == null)
		//		{
		//			game.Division = _context.Divisions.Find(game.DivisionID);
		//		}
		//		ViewData["DivisionID"] = DivisionSelectionList(game.Division.ID);
		//		ViewData["HomeTeamID"] = TeamSelectionList(game.TeamGame.HomeTeam?.ID,game.Division.ID);
		//		ViewData["AwayTeamID"] = TeamSelectionList(game.TeamGame.AwayTeam?.ID, game.Division.ID);
		//	}
		//	else
		//	{
		//		ViewData["DivisionID"] = DivisionSelectionList(null);
		//		ViewData["HomeTeamID"] = TeamSelectionList(null, 0);
		//		ViewData["AwayTeamID"] = TeamSelectionList(null, 0);
		//	}
		//}
		

        private void PopulateDropDownList(Game game)
        {
            var allTeams = _context.Teams.ToList();

            // Check if TeamGame is null
            if (game.TeamGame == null)
            {
                ViewData["Teams"] = new SelectList(allTeams, "ID", "TeamName");
                ViewData["SelectedHomeTeam"] = null;
                ViewData["SelectedAwayTeam"] = null;
                return;
            }

            // Populate selected home and away teams
            var selectedHomeTeam = game.TeamGame.HomeTeam?.ID;
            var selectedAwayTeam = game.TeamGame.AwayTeam?.ID;

            ViewData["Teams"] = new SelectList(allTeams, "ID", "TeamName");
            ViewData["SelectedHomeTeam"] = selectedHomeTeam;
            ViewData["SelectedAwayTeam"] = selectedAwayTeam;
        }
		//[HttpGet]
		//public JsonResult GetCities(int DivisionID)
		//{
		//	return Json(TeamSelectionList(DivisionID, 0));
		//}
		private bool GameExists(int id) => _context.Games.Any(e => e.ID == id);
    }
}

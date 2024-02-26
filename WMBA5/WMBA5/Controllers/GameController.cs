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

        // GET: Game/Create
        public IActionResult Create()
        {
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
            ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name");
            return View();
        }

		//// GET: Game/AddTeam/5
		//public async Task<IActionResult> AddTeams(int? id)
		//{
		//	if (id == null)
		//	{
		//		return NotFound();
		//	}

		//	var game = await _context.Games
		//		.Include(g => g.Division)
  //              .Include(g => g.AwayTeam)
  //              .Include(g => g.HomeTeam)
  //              .FirstOrDefaultAsync(m => m.ID == id);

		//	if (game == null)
		//	{
		//		return NotFound();
		//	}

		//	// Filter teams based on DivisionID of the game
		//	var teamsInSameDivision = _context.Teams.Where(t => t.DivisionID == game.DivisionID);

  //          ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
  //          ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name"); ;

		//	return View(game);
		//}

		//// POST: Game/AddTeam/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> AddTeams(int id, [Bind("ID")] Game gameViewModel, int selectedHomeTeam, int selectedAwayTeam)
		//{
		//	var gameToUpdate = await _context.Games.Include(g => g.AwayTeam)
  //              .Include(g => g.HomeTeam).FirstOrDefaultAsync(g => g.ID == id);

		//	if (gameToUpdate == null)
		//	{
		//		return NotFound();
		//	}

		//	if (ModelState.IsValid)
		//	{
		//		// Update the TeamGame for the game
		//		if (gameToUpdate.TeamGame == null)
		//		{
		//			gameToUpdate.TeamGame = new TeamGame();
		//		}
		//		gameToUpdate.TeamGame.HomeTeamID = selectedHomeTeam;
		//		gameToUpdate.TeamGame.AwayTeamID = selectedAwayTeam;

		//		try
		//		{
		//			await _context.SaveChangesAsync();
		//		}
		//		catch (DbUpdateConcurrencyException)
		//		{
		//			if (!GameExists(gameToUpdate.ID))
		//			{
		//				return NotFound();
		//			}
		//			else
		//			{
		//				throw;
		//			}
		//		}
		//		return RedirectToAction(nameof(Index));
		//	}

		//	// If we got this far, something failed, redisplay form
		//	return View(gameViewModel);
		//}


		// POST: Game/Create
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StartTime,LocationID,OutcomeID, DivisionID")] Game game)/* int selectedHomeTeam, int selectedAwayTeam)*/
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Load all of the Teams with Players into the game object first
                    game.HomeTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.HomeTeamID);
                    game.AwayTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.AwayTeamID);

                    //Set the initial lineups with all team members
                    FillLineupsWithTeams(game);

                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = game.ID });
                }
                ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
                ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name");
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
                
            }

            catch(Exception ex) 
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
               .Include(d => d.Division)
               .FirstOrDefaultAsync(d => d.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
            ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            return View(game);
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StartTime,LocationID,OutcomeID, DivisionID")] Game game, int selectedHomeTeam, int selectedAwayTeam)
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
                        ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionSummary", game.DivisionID);
                        ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
                        ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name");
                        return View(game);
                    }
                    // Find the existing game in the database
                    var existingGame = await _context.Games
                        .Include(g => g.AwayTeam)
                        .Include(g => g.HomeTeam)
                        .FirstOrDefaultAsync(m => m.ID == id);

                    if (existingGame == null)
                    {
                        return NotFound();
                    }

                    // Update the game properties
                    existingGame.DivisionID = game.DivisionID;

                    //// Update the associated TeamGame
                    //if (existingGame.TeamGame == null)
                    //{
                    //    existingGame.TeamGame = new TeamGame();
                    //}

                    //existingGame.TeamGame.HomeTeamID = selectedHomeTeam;
                    //existingGame.TeamGame.AwayTeamID = selectedAwayTeam;

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
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "Name");
            ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "Name");
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



        //Selecting the division for game

        //Selecting Teams that are part of the division we select
        //private SelectList TeamSelectionList(int? selectedId, int DivisionID)
        //{
        //	var query = from t in _context.Teams
        //				where t.DivisionID == DivisionID
        //				select t;
        //	return new SelectList(query.OrderBy(p => p.TeamName), "ID", "TeamSummary", selectedId);


        //}
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

      

   //     private void PopulateDropDownList(Game game)
   //     {
			//ViewData["OutcomeID"] = OutcomeSelectionList(game?.OutcomeID);
			//ViewData["DivisionID"] = DivisionSelectionList(game?.DivisionID);
			//ViewData["LocationID"] = LocationSelectionList(game?.LocationID);
			//var allTeams = _context.Teams.ToList();

   //         // Check if TeamGame is null
   //         if (game.TeamGame == null)
   //         {
   //             ViewData["Teams"] = new SelectList(allTeams, "ID", "TeamName");
             
   //             ViewData["SelectedHomeTeam"] = null;
   //             ViewData["SelectedAwayTeam"] = null;
   //             return;
   //         }

   //         // Populate selected home and away teams
   //         var selectedHomeTeam = game.TeamGame.HomeTeam?.ID;
   //         var selectedAwayTeam = game.TeamGame.AwayTeam?.ID;

   //         ViewData["Teams"] = new SelectList(allTeams, "ID", "TeamName");
   //         ViewData["SelectedHomeTeam"] = selectedHomeTeam;
   //         ViewData["SelectedAwayTeam"] = selectedAwayTeam;
   //     }
        //[HttpGet]
        //public JsonResult GetCities(int DivisionID)
        //{
        //	return Json(TeamSelectionList(DivisionID, 0));
        //}

        [HttpGet]
        public JsonResult GetLocations(int? id)
        {
            return Json(LocationSelectionList(id));
        }
        private bool GameExists(int id) => _context.Games.Any(e => e.ID == id);
    }
}

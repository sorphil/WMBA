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
using WMBA5.ViewModels;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.DiaSymReader;
using Org.BouncyCastle.Utilities.IO;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor, Trash Pandas 15U Coach, Trash Pandas 15U Scorekeeper, Scorekeeper, Coach")]
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
            //Filter for  Trash Pandas 15U Coach or Scorekeeper
            if (User.IsInRole("Trash Pandas 15U Coach") || User.IsInRole("Trash Pandas 15U Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Trash Pandas" || t.AwayTeam.TeamName == "Trash Pandas" && t.Division.DivisionName == "15U");
            }
            //Filter for Rookie Convenor
            if (User.IsInRole("Rookie Convenor"))
            {
                //1 is the ID for U9
                gamesQuery = gamesQuery.Where(t => t.DivisionID == 1);
            }
            //Filter for Intermeditate Convenor
            if (User.IsInRole("Intermediate Convenor"))
            {
                //2 is the ID for U11 and 3 for U13
                gamesQuery = gamesQuery.Where(t => t.DivisionID == 2 || t.DivisionID == 3);
            }
            //Filter for senior Convenor
            if (User.IsInRole("Senior Convenor"))
            {
                //4 is the ID for U15
                gamesQuery = gamesQuery.Where(t => t.DivisionID >= 4);
            }
            //Rest of filters options
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

        //Get: Game/Create/5
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
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
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
        public async Task<IActionResult> Create([Bind("ID,StartTime,HomeTeamID,AwayTeamID,LocationID")] Game game, int HomeTeamID, int AwayTeamID)
        {
            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            try
            {
                var team = await _context.Teams.Where(t => t.ID == HomeTeamID).FirstOrDefaultAsync();
                var outcome = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "TBD");
                game.DivisionID = team.DivisionID;
                game.OutcomeID = outcome.ID;
                if (ModelState.IsValid)
                {
                    //Load all of the Teams with Players into the game object first
                    game.HomeTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.HomeTeamID);
                    game.AwayTeam = _context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.AwayTeamID);

                    //Set the initial lineups with all team members
                    FillLineupsWithTeams(game);

                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        error.ToString();
                    }
                }
                ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
                ViewData["AwayTeamID"] = teamList;
                ViewData["HomeTeamID"] = teamList;
                ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");

            }

            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(game);
        }

        [HttpGet]
        public async Task<JsonResult>GetTeamsByDivision(int divisionId)
        {
            // Retrieve teams for the selected division
            var teams = await _context.Teams
                .Where(t => t.DivisionID == divisionId)
                .Select(t => new SelectListItem { Value = t.ID.ToString(), Text = $"{t.Division.DivisionName} - {t.TeamName}" })
                .ToListAsync();

            return Json(teams);
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
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            // Keeping your original SelectList assignments for other fields
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["OutcomeID"] = new SelectList(_context.Outcomes, "ID", "OutcomeString", game.OutcomeID);

            // Constructing the team list with divID as a string before the team names
            var teamList = _context.Teams.Where(t => t.DivisionID == game.DivisionID).Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = $"{t.Division.DivisionName} - {t.TeamName}" // Adjusting format to include divID
            });

            // Using the constructed teamList for both AwayTeamID and HomeTeamID dropdowns
            ViewData["HomeTeamID"] = new SelectList(teamList, "Value", "Text", game.HomeTeamID); // Set selected home team
            ViewData["AwayTeamID"] = new SelectList(teamList, "Value", "Text", game.AwayTeamID); // Set selected away team

            return View(game);
        }


        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
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
        [HttpGet]
        //Creating the action to record the in-game Stats for futher creation of the view
        public async Task<IActionResult> InGameStatsRecord(int? id, string LineupStr = "Home")
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameStats = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g => g.Outcome)
                .Include(g => g.Location)
                .Include(g => g.Innings).ThenInclude(g => g.Scores)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (gameStats == null)
            {
                return NotFound();
            }

            ViewBag.GameID = id;

            // Check if there are any players in the selected lineup (Home/Away)
            var lineup = LineupStr.Equals("Home", StringComparison.OrdinalIgnoreCase) ? TeamLineup.Home : TeamLineup.Away;
            var lineupIsEmpty = gameStats.GamePlayers.Any(gp => gp.BattingOrder == 0 && gp.TeamLineup == TeamLineup.Home);
            var playerCount = gameStats.GamePlayers.Count(gp => gp.BattingOrder != 0 && gp.TeamLineup == TeamLineup.Home);

            if (lineupIsEmpty)
            {
                return RedirectToAction(nameof(EditLineup), new { id = id, Lineup = LineupStr });
            }

            if (playerCount < 8)
            {
                TempData["ShowLoseEditLineupModal"] = true; // Set flag to show the modal
                return RedirectToAction(nameof(Index), new { id = id, Lineup = LineupStr });
            }

            if (!gameStats.Innings.Any())
            {
                var inning = new Inning
                {
                    GameID = gameStats.ID,
                    InningNo = $"Inning {(gameStats.Innings.Count() + 1).ToString()}",
                };
                _context.Innings.Add(inning);
                await _context.SaveChangesAsync();

                gameStats.Innings.Add(inning);
                ViewBag.Inning = inning;
                ViewBag.Innings = gameStats.Innings;
            }
            else
            {
                var innings = _context.Innings.Where(i => i.GameID == gameStats.ID).ToList();
                ViewBag.Innings = innings;
            }

            var teamScores = _context.Scores.Include(s => s.Player).ThenInclude(p => p.Team)
                .GroupBy(s => new { TeamID = s.Player.TeamID, s.GameID })
                .Select(g => new TeamScoreVM
                {
                    TeamID = g.Key.TeamID.GetValueOrDefault(),
                    GameID = g.Key.GameID,
                    TotalRuns = g.Sum(s => s.Runs)
                }).FirstOrDefault();

            ViewBag.TotalRuns = teamScores?.TotalRuns ?? 0;
            ViewBag.TeamLineup = LineupStr;

            return View(gameStats);
        }

        [HttpPost]
        public async Task<IActionResult> InGameStatsRecord(int? id, int? PlayerID, int? InningID, int? GameID, string? IncrementField, int? IncrementValue)
        {
            int? gameID = ViewBag.GameID;
            // Find or create the score object for the player, inning, and game
            var score = await _context.Scores.FirstOrDefaultAsync(s => s.PlayerID == PlayerID && s.InningID == InningID && s.GameID == GameID);
            
            if (score == null)
            {
                // Create a new score object if it doesn't exist
                score = new Score
                {
                    PlayerID = (int)PlayerID,
                    InningID = (int)InningID,
                    GameID = (int)GameID
                };
                _context.Scores.Add(score);
            }
            // Increment the appropriate field based on the IncrementField parameter
            switch (IncrementField)
            {
                case "Hits":
                    if(IncrementValue.GetValueOrDefault()<0 && score.Runs==0)
                    {
                        break;
                    }
                   else if(IncrementValue.GetValueOrDefault()!=null|| IncrementValue.GetValueOrDefault() != 0)
                    {
                        //score.Hits += IncrementValue.GetValueOrDefault();
                    }
                    break;
                case "Balls":
                    if (IncrementValue.GetValueOrDefault() < 0 && score.Balls == 0)
                    {
                        break;
                    }
                    else if (IncrementValue.GetValueOrDefault() != null || IncrementValue.GetValueOrDefault() != 0)
                    {
                        score.Balls += IncrementValue.GetValueOrDefault();
                    }
                    break;
                case "Strikes":
                    if (IncrementValue.GetValueOrDefault() < 0 && score.Strikes == 0)
                    {
                        break;
                    }
                 
                    if (score.Strikes + IncrementValue.GetValueOrDefault() >= 3)
                    {
                        //score.Out = score.Out + 1;
                        score.Strikes = 0;
                    }
                    else if (IncrementValue.GetValueOrDefault() != null || IncrementValue.GetValueOrDefault() != 0 || score.Strikes + IncrementValue.GetValueOrDefault() < 3)
                    {
                        score.Strikes += IncrementValue.GetValueOrDefault();
                    }
                    
                    break;
                    case "Outs":
                    if (IncrementValue.GetValueOrDefault() < 0 && score.Outs == 0)
                    {
                        break;
                    }
                    else if (IncrementValue.GetValueOrDefault() != null || IncrementValue.GetValueOrDefault() != 0)
                    {
                        //score.Out += IncrementValue.GetValueOrDefault();
                    }
                    break;
                case "Runs":
                    if (IncrementValue.GetValueOrDefault() < 0 && score.Runs == 0)
                    {
                        break;
                    }
                    else if (IncrementValue.GetValueOrDefault() != null || IncrementValue.GetValueOrDefault() != 0)
                    {
                        score.Runs += IncrementValue.GetValueOrDefault();
                    }

                    break;
            }
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to appropriate action or view
            return Json(score);
        }
        [HttpGet]
        public async Task<IActionResult> ChangeTeam(int? id, string LineupStr)
        {
            var gameStats = await _context.Games
                   .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                   .Include(g => g.HomeTeam)
                   .Include(g=>g.AwayTeam)
                      .FirstOrDefaultAsync(m => m.ID == id);
            var players = await _context.GamePlayers.Include(gp => gp.Player).Where(gp => gp.TeamLineup == TeamLineup.Away && gp.GameID == id).OrderBy(gp => gp.BattingOrder).AsNoTracking().ToListAsync();
            if (LineupStr=="Home")
            {

                players = await _context.GamePlayers.Include(gp => gp.Player)
                    .Where(gp => gp.TeamLineup == TeamLineup.Home && gp.GameID == id).OrderBy(gp=>gp.BattingOrder).AsNoTracking().ToListAsync();

                return Json(players);

            }

            return Json(players);



        }
        [HttpGet]
        public async Task<IActionResult> GetPlayerScore(int? GameID, int? PlayerID, int? InningID)
        {
            var playerScore = await _context.Scores
                .FirstOrDefaultAsync(s => s.PlayerID == PlayerID && s.InningID == InningID && s.GameID == GameID);


            if (playerScore == null)
            {
                // Create a new score object if it doesn't exist
                var score = new Score
                {
                    PlayerID = (int)PlayerID,
                    InningID = (int)InningID,
                    GameID = (int)GameID,
                    Balls = 0,
                    Runs = 0,
                    FoulBalls = 0,
                    //Hits = 0,
                    Strikes = 0,
                    //Outs = 0
                };
                _context.Scores.Add(score);
                await _context.SaveChangesAsync();
                return Json(score);
            }

            return Json(playerScore);
        }
        // GET: Game/Delete/5
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
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
        [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
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

        // GET: Game/EditLineup/5
        public async Task<IActionResult> EditLineup(int id, string Lineup)
        {
            //Convert back to the Enum for Lineup
            Enum.TryParse(Lineup, out TeamLineup lineup);

            var game = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player).ThenInclude(t => t.Team)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            PopulateAssignedLineupData(game, lineup);
            ViewData["Lineup"] = lineup.ToString();
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLineup(int? id, string[] selectedOptions, string Lineup)
        {
            //Convert back to the Enum for Lineup
            Enum.TryParse(Lineup, out TeamLineup lineup);

            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player).ThenInclude(t => t.Team)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            //Remove the one Lineup from the game
            foreach (GamePlayer gp in game.GamePlayers)
            {
                if (gp.TeamLineup == lineup)
                {
                    game.GamePlayers.Remove(gp);
                }
            }
            //Add them back but in order they were in the listbox
            int i = 1;
            foreach (string selected in selectedOptions)
            {
                game.GamePlayers.Add(new GamePlayer()
                {
                    PlayerID = int.Parse(selected),
                    GameID = game.ID,
                    BattingOrder = i,
                    TeamLineup = lineup
                });
                i++;
            }
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { game.ID });
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(game);
        }

        private void PopulateAssignedLineupData(Game game, TeamLineup lineup)
        {
            //For this to work, you must have Included the child collection in the parent object
            //List of IDs of all players in the game
            List<int> playersInGame = game.GamePlayers.Select(x => x.PlayerID).ToList();
            //Now we can get all of the other players
            var allOptions = _context.Players
                .Include(p => p.Team)
                .Where(p => !playersInGame.Contains(p.ID) && p.TeamID == game.HomeTeamID)
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

            //Current players on the lineup
            var currentLineup = game.GamePlayers
                .Where(gp => gp.TeamLineup == lineup)
                .OrderBy(gp => gp.BattingOrder)
                .ThenBy(gp => gp.Player.FullName);

            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            foreach (var lineupPlayer in currentLineup)
            {
                // Check if BattingOrder is greater than 0 before adding it to the DisplayText
                string displayText = lineupPlayer.BattingOrder > 0 ? lineupPlayer.BattingOrder.ToString() + " - " + lineupPlayer.Player.Summary : lineupPlayer.Player.Summary;

                selected.Add(new ListOptionVM
                {
                    ID = lineupPlayer.PlayerID,
                    DisplayText = displayText
                });
            }
            var available = new List<ListOptionVM>();
            foreach (var player in allOptions)
            {
                available.Add(new ListOptionVM
                {
                    ID = player.ID,
                    DisplayText = player.Summary
                });
            }

            ViewData["selOpts"] = new MultiSelectList(selected, "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available, "ID", "DisplayText");

    }

        [HttpPost]
        public async Task<IActionResult> NewInning(int? id)
        {
            int? gameID = ViewBag.GameID;
            var game = await _context.Games.FirstOrDefaultAsync(g => g.HomeTeam.ID == id);

            try
            {
                var inning = new Inning
                {
                    GameID = game.ID,
                    InningNo = $"Inning {(_context.Innings.Where(i => i.GameID == id).ToList().Count() + 1).ToString()}",
                };
                _context.Innings.Add(inning);
                await _context.SaveChangesAsync();
                ViewBag.Inning = inning;

                return Json(inning);
            }
            catch (Exception ex)
            {

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

        [HttpPost]
        public async Task<IActionResult> NewScoreObject(int? GameID, int? PlayerID, int? InningID)
        {
            var score = await _context.Scores.FirstOrDefaultAsync(s => s.PlayerID == PlayerID && s.InningID == InningID && s.GameID == GameID);
            if (score == null)
            {
                // Create a new score object if it doesn't exist
                score = new Score
                {
                    PlayerID = (int)PlayerID,
                    InningID = (int)InningID,
                    GameID = (int)GameID
                };
                _context.Scores.Add(score);
                await _context.SaveChangesAsync();
            }
            var scoreJson = JsonConvert.SerializeObject(
                new
                    {
                        score.ID,
                        score.Balls,
                        score.FoulBalls,
                        score.Strikes,
                        score.Outs,
                        score.Runs,
                        score.Hits,
                        score.InningID,
                        score.GameID,
                        score.PlayerID
                    }
                );

            return Json(scoreJson);

        }

        [HttpGet]
        //Creating the action to record the in-game Stats for futher creation of the view
        public async Task<IActionResult> GetGameInfo(int? id)
        {
            var gameStats = await _context.Games
         .Include(g => g.GamePlayers).ThenInclude(gp => gp.Player).ThenInclude(p => p.Scores)
         .Include(g => g.Runners).ThenInclude(r => r.Player)
         .FirstOrDefaultAsync(m => m.ID == id);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var gameStatsJson = JsonConvert.SerializeObject(gameStats, settings);
            // Serialize gameStats to JSON


            return Json(gameStatsJson);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGameInfo(string json)
        {
            Console.WriteLine(json);
            JObject jObject = JObject.Parse(json);
            var gameStats = JsonConvert.DeserializeObject<Game>(json.ToString());
            //_context.Games.Update(gameStats);


            var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.ID == gameStats.ID);
            if (existingGame != null)
            {

                // Detach the existing Game entity from the context
                _context.Entry(existingGame).State = EntityState.Detached;
                // Update properties of existingGame with values from gameStats
                existingGame.GamePlayers = gameStats.GamePlayers;
                existingGame.Innings = gameStats.Innings;

                // Repeat for other properties as needed
                _context.Update(existingGame);
                _context.SaveChanges();
            }
            foreach (var score in gameStats.Scores)
            {
                _context.Entry(score).State = EntityState.Detached;
            }

            return Json(new { success = true, data = gameStats });
        }
    }
}
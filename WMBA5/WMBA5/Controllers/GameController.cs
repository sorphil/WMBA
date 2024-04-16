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
using System.Diagnostics;
using Org.BouncyCastle.Utilities.Collections;

namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor,Scorekeeper, Coach")]
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
            #region Filter for Coaches and Scorekeepers
            //Filter for Trash Pandas 15U Coach
            if (User.IsInRole("Trash Pandas - 15U - Coach") || User.IsInRole("Trash Pandas - 15U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Trash Pandas" || t.AwayTeam.TeamName == "Trash Pandas" && t.Division.DivisionName == "15U");
            }
            //Filter for Bananas - 13U - Coach
            if (User.IsInRole("Bananas - 13U - Coach")|| User.IsInRole("Bananas - 13U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Bananas" || t.AwayTeam.TeamName == "Bananas" && t.Division.DivisionName == "13U");
            }
            //Filter for Iron Birds - 13U - Coach
            if (User.IsInRole("Iron Birds - 13U - Coach") || User.IsInRole("Iron Birds - 13U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Iron Birds" || t.AwayTeam.TeamName == "Iron Birds" && t.Division.DivisionName == "13U");
            }
            //Filter for Whitecaps - 13U - Coach
            if (User.IsInRole("Whitecaps - 13U - Coach") || User.IsInRole("Whitecaps - 13U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Whitecaps" || t.AwayTeam.TeamName == "Whitecaps" && t.Division.DivisionName == "13U");
            }
            //Filter for Bisons - 15U - Coach  
            if (User.IsInRole("Bisons - 15U - Coach") || User.IsInRole("Bisons - 15U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Bisons" || t.AwayTeam.TeamName == "Bisons" && t.Division.DivisionName == "15U");
            }
            //Filter for Dragons - 15U - Coach
            if (User.IsInRole("Dragons - 15U - Coach") || User.IsInRole("Dragons - 15U - Scorekeeper"))
            {
                //1 is the ID for U9
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Dragons" || t.AwayTeam.TeamName == "Dragons" && t.Division.DivisionName == "15U");
            }
            //Filter for Raimon - 9U - Coach   
            if (User.IsInRole("Raimon - 9U - Coach") || User.IsInRole("Raimon - 9U - Scorekeeper"))
            {
                //1 is the ID for U9
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Raimon" || t.AwayTeam.TeamName == "Raimon" && t.Division.DivisionName == "9U");
            }
            //Orfeo - 9U - Coach
            if (User.IsInRole("Orfeo - 9U - Coach") || User.IsInRole("Orfeo - 9U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Orfeo" || t.AwayTeam.TeamName == "Orfeo" && t.Division.DivisionName == "9U");
            }
            //Filter for Trash Pandas 15U Coach
            if (User.IsInRole("Kings - 9U - Coach") || User.IsInRole("Kings - 9U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Kings" || t.AwayTeam.TeamName == "Kings" && t.Division.DivisionName == "9U");
            }
            //Filter for Trash Pandas - 11U - Coach  
            if (User.IsInRole("Trash Pandas - 11U - Coach") || User.IsInRole("Trash Pandas - 11U - Scorekeeper"))
            {
                //1 is the ID for U9
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Trash Pandas" || t.AwayTeam.TeamName == "Trash Pandas" && t.Division.DivisionName == "11U");
            }
            //Angry Birds - 11U - Coach 
            if (User.IsInRole("Angry Birds - 11U - Coach") || User.IsInRole("Angry Birds - 11U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Angry Birds" || t.AwayTeam.TeamName == "Angry Birds" && t.Division.DivisionName == "11U");
            }
            //Filter for G2 - 11U - Coach  
            if (User.IsInRole("G2 - 11U - Coach") || User.IsInRole("G2 - 11U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "G2" || t.AwayTeam.TeamName == "G2" && t.Division.DivisionName == "11U");
            }
            //Raimon - 18U - Coach   
            if (User.IsInRole("Raimon - 18U - Coach") || User.IsInRole("Raimon - 18U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Raimon" || t.AwayTeam.TeamName == "Raimon" && t.Division.DivisionName == "18U");
            }
            //Leviathan - 18U - Coach 
            if (User.IsInRole("Leviathan - 18U - Coach") || User.IsInRole("Leviathan - 18U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Leviathan" || t.AwayTeam.TeamName == "Leviathan" && t.Division.DivisionName == "18U");
            }
            //Orfeo - 9U - Coach
            if (User.IsInRole("Wellanders - 18U - Coach") || User.IsInRole("Wellanders - 18U - Scorekeeper"))
            {
                gamesQuery = gamesQuery.Where(t => t.HomeTeam.TeamName == "Wellanders" || t.AwayTeam.TeamName == "Wellanders" && t.Division.DivisionName == "18U");
            }
            #endregion
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
                .Include(g => g.Outcome)
                .Include(g => g.Location)
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
        public async Task<JsonResult> GetTeamsByDivision(int divisionId)
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
        public async Task<IActionResult> InGameStatsRecord(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameStats = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                .Include(g => g.PlayerAtBat)
                .Include(g => g.Runners)
                .Include(g => g.Outcome)
                .Include(g => g.CurrentInning)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g => g.Outcome)
                .Include(g => g.Location)
                .Include(g => g.Innings).ThenInclude(g => g.Scores)

                .FirstOrDefaultAsync(m => m.ID == id);

            if (gameStats == null)
            {
                return NotFound();
            }

            ViewBag.GameID = id;


            var lineupIsEmpty = gameStats.GamePlayers.Any(gp => gp.BattingOrder == 0 && gp.TeamLineup == TeamLineup.Home);
            var playerCount = gameStats.GamePlayers.Count(gp => gp.BattingOrder != 0 && gp.TeamLineup == TeamLineup.Home);

            var outcomeWinAway = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Win-Away");
            var outcomeWinHome = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Win-Home");
            var outcomeTie = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Tie");
            var gameOutcome = gameStats.OutcomeID;
            if (gameOutcome == outcomeWinAway.ID || gameOutcome == outcomeWinHome.ID)
            {
                return RedirectToAction(nameof(InGameStats), new { id = id });
            }
            if (lineupIsEmpty)
            {
                return RedirectToAction(nameof(EditLineup), new { id = id });
            }

            if (playerCount < 8)
            {
                TempData["ShowLoseEditLineupModal"] = true; // Set flag to show the modal
                return RedirectToAction(nameof(Index), new { id = id });
            }
            ViewBag.AtBat = gameStats.PlayerAtBat;

            if (gameStats.PlayerAtBatID == null)
            {
                var GamePlayer = await _context.GamePlayers.Include(gp => gp.Player)
                                    .FirstOrDefaultAsync(gp => gp.BattingOrder == 1 && gp.GameID == id && gp.TeamLineup == 0);
                ViewBag.AtBat = GamePlayer.Player;
                gameStats.PlayerAtBatID = GamePlayer?.PlayerID;

            }


            if (!gameStats.Innings.Any())
            {
                for (var i = 0; gameStats.Division.DivisionName == "9U" ? i < 7 : i < 9; i++)
                {
                    var inning = new Inning
                    {
                        GameID = gameStats.ID,
                        InningNo = $"Inning {(gameStats.Innings.Count() + 1).ToString()}",
                    };
                    _context.Innings.Add(inning);
                    gameStats.Innings.Add(inning);
                }
                await _context.SaveChangesAsync();

                ViewBag.Innings = gameStats.Innings;
            }
            else
            {
                var innings = _context.Innings.Where(i => i.GameID == gameStats.ID).ToList();
                ViewBag.Innings = innings;
                ViewBag.CurrentInning = gameStats.CurrentInningID;
            }
            ViewBag.CurrentInning = gameStats.CurrentInningID;
            if (gameStats.CurrentInningID == null || gameStats.CurrentInningID == 0)
            {
                var currentInning = await _context.Innings.FirstOrDefaultAsync(i => i.GameID == id && i.InningNo == "Inning 1");
                ViewBag.CurrentInning = currentInning?.ID;
                gameStats.CurrentInning = currentInning;
                gameStats.CurrentInningID = currentInning.ID;
            }
            if (!gameStats.Runners.Any())
            {
                for (var i = 1; i <= 3; i++)
                {
                    var runner = new Runner
                    {
                        GameID = gameStats.ID,
                        Base = (Base)i,
                    };
                    _context.Runners.Add(runner);
                    gameStats.Runners.Add(runner);
                }
                await _context.SaveChangesAsync();
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
            ViewBag.TeamLineup = "Home";


            return View(gameStats);
        }
        [HttpGet]
        //Creating the action to record the in-game Stats for futher creation of the view
        public async Task<IActionResult> InGameStats(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameStats = await _context.Games
                .Include(g => g.GamePlayers).ThenInclude(p => p.Player)
                .Include(g => g.PlayerAtBat)
                .Include(g => g.Runners)
                .Include(g => g.Outcome)
                .Include(g => g.CurrentInning)
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.Division)
                .Include(g => g.Outcome)
                .Include(g => g.Location)
                .Include(g => g.Innings).ThenInclude(g => g.Scores)

                .FirstOrDefaultAsync(m => m.ID == id);

            if (gameStats == null)
            {
                return NotFound();
            }

            if (gameStats.Outcome.OutcomeString == "Win-Away")
            {
                ViewBag.Winner = gameStats.AwayTeam.TeamName;
            }
            else if (gameStats.Outcome.OutcomeString == "Win-Home")
            {
                ViewBag.Winner = gameStats.HomeTeam.TeamName;
            }

            ViewBag.GameID = id;
            ViewBag.Innings = gameStats.Innings;
            ViewBag.Players = gameStats.GamePlayers.Where(gp => gp.TeamLineup == 0).OrderBy(gp => gp.BattingOrder).ToList();


            return View(gameStats);
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
                .Include(g => g.Outcome)
                .Include(g => g.Location)
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
                    score.Singles,
                    score.Doubles,
                    score.Triples,
                    score.InningID,
                    score.GameID,
                    score.PlayerID,
                    score.Hits,
                    score.Walks,
                    score.StrikeOuts,
                    score.FlyOuts,
                    score.GroundOuts
                }
                );

            return Json(scoreJson);

        }


        [HttpPost]
        public async Task<IActionResult> NewInningObject(int? GameID, int InningNo)
        {


            var inning = new Inning
            {
                GameID = GameID.GetValueOrDefault(),
                AwayRuns = null,
                InningNo = $"Inning {InningNo}"
            };
            _context.Innings.Add(inning);
            await _context.SaveChangesAsync();
            var inningJson = JsonConvert.SerializeObject(
               new
               {
                   inning.ID,
                   inning.GameID,
                   inning.AwayRuns,
                   inning.InningNo
               });
            var gameStats = await _context.Games.FirstOrDefaultAsync(m => m.ID == GameID && m.GamePlayers.Any(gp => gp.TeamLineup == 0));
            var outcomeTie = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Tie");
            gameStats.OutcomeID = outcomeTie.ID;
            await _context.SaveChangesAsync();
            return Json(inningJson);

        }
        [HttpGet]
        //Creating the action to record the in-game Stats for further creation of the view
        public async Task<IActionResult> GetGameInfo(int? id)
        {
            var gameStats = await _context.Games
        .Include(g => g.GamePlayers)
            .ThenInclude(gp => gp.Player)
                .ThenInclude(p => p.Scores)
        .Include(g => g.CurrentInning)
        .Include(g => g.Innings)
        .Include(g => g.Runners)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.ID == id && m.GamePlayers.Any(gp => gp.TeamLineup == 0));
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 20 // Set your desired maximum depth value
            };
            var gameStatsJson = JsonConvert.SerializeObject(gameStats, settings);
            // Serialize gameStats to JSON


            return Json(gameStatsJson);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGameInfo(string json)
        {

            JObject jObject = JObject.Parse(json);
            var gameStats = JsonConvert.DeserializeObject<Game>(json.ToString());
            //_context.Games.Update(gameStats);


            var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.ID == gameStats.ID);
            if (existingGame != null)
            {

                // Detach the existing Game entity from the context
                _context.Entry(existingGame).State = EntityState.Detached;

                existingGame.PlayerAtBatID = gameStats.PlayerAtBatID;
                existingGame.CurrentInningID = gameStats.CurrentInningID;

                // Repeat for other properties as needed
                _context.Update(existingGame);
                _context.SaveChanges();
            }

            return Json(new { success = true, data = gameStats });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGameRunners(string json)
        {

            try
            {

                // Parse the JSON string into a JArray
                JArray jArray = JArray.Parse(json);
                ;
              
                foreach (var runnerJson in jArray)
                {
                    Debug.WriteLine(runnerJson);
                    var runner = JsonConvert.DeserializeObject<Runner>(runnerJson.ToString());
                    // Assuming you have access to your DbContext instance (_context)
                    _context.Runners.Update(runner);
                }
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return appropriate response
                return Ok("Runners added successfully.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateGameInnings(string json)
        {
            try
            {

                // Parse the JSON string into a JArray
                JArray jArray = JArray.Parse(json);

                foreach (var inningJson in jArray)
                {

                    var inning = JsonConvert.DeserializeObject<Inning>(inningJson.ToString());
                    // Assuming you have access to your DbContext instance (_context)
                    _context.Innings.Update(inning);
                }
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return appropriate response
                return Ok("Innings added successfully.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateGamePlayers(string json)
        {
            try
            {

                // Parse the JSON string into a JArray
                JArray jArray = JArray.Parse(json);

                foreach (var playersJson in jArray)
                {
                    var player = JsonConvert.DeserializeObject<GamePlayer>(playersJson.ToString());
                    _context.Entry(player).State = EntityState.Detached;
                    if (player.Player != null)
                    {
                        _context.Entry(player.Player).State = EntityState.Detached;
                        if (player.Player.GamePlayers.Any())
                        {
                            foreach (var playerGamePlayer in player.Player.GamePlayers)
                            {
                                _context.Entry(playerGamePlayer).State = EntityState.Detached;
                            }
                        }
                        if(player.Player.Scores.Any())
                        {
                            foreach (var score in player.Player.Scores)
                            {
                                _context.Entry(score).State = EntityState.Detached;
                            }    
                        }
                    }
                    Debug.WriteLine("ASDASDASDASDASDA");
                    Debug.WriteLine(player);
                    // Assuming you have access to your DbContext instance (_context)
                    _context.GamePlayers.Update(player);
                }
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return appropriate response
                return Ok("Players added successfully.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IActionResult> EndGame(int? id, string winner)
        {
            var gameStats = await _context.Games.FirstOrDefaultAsync(m => m.ID == id && m.GamePlayers.Any(gp => gp.TeamLineup == 0));
            var outcomeTBD = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "TBD");
            var outcomeWinAway = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Win-Away");
            var outcomeWinHome = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Win-Home");
            var outcomeTie = await _context.Outcomes.FirstOrDefaultAsync(o => o.OutcomeString == "Tie");
            //var gameOutcome = gameStats.OutcomeID;

            if (winner == "Home")
            {
                gameStats.OutcomeID = outcomeWinHome.ID;
            }
            else if (winner == "Away")
            {
                gameStats.OutcomeID = outcomeWinAway.ID;
            }
            await _context.SaveChangesAsync();
            // Assuming you have the redirect URL stored in a variable named redirectUrl
            var redirectUrl = Url.Action(nameof(InGameStats), new { id = id });

            // Return JSON response containing the redirect URL
            return Json(new { redirectUrl });

        }



        [HttpGet]
        public async Task<IActionResult> GetGameInnings(int id)
        {
            var innings = await _context.Innings.Include(i => i.Scores)
                .Where(i => i.GameID == id)
                .ToListAsync();

            return Json(innings);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WMBA5.Models;

namespace WMBA5.Data
{
    public static class WMBAInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            WMBAContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<WMBAContext>();
            try
            {
                //Delete and recreate the database. Just for testing phase.
                context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                context.Database.Migrate();

                //To randomly generate data
                Random random = new Random();
                //context.Database.Migrate();
                //Add the parent table first before adding child table.

                //Adding Clubs First
                if (!context.Clubs.Any())
                {
                    context.Clubs.Add(
                       new Club
                       {
                           ClubName = "Welland Minor Baseball Association"
                       });
                    context.SaveChanges();
                }
                //Adding Statuses Next
                if (!context.Statuses.Any())
                {
                    context.Statuses.AddRange(
                       new Status
                       {
                           StatusName = "Active"
                       },
                       new Status
                       {
                            StatusName = "Inactive"
                       },
                       new Status
                       {
                           StatusName = "Injured"
                       });
                    context.SaveChanges();
                }

                //Adding Coach Next
                if (!context.Coaches.Any())
                {
                    context.Coaches.AddRange(
                       new Coach
                       {
                           CoachName = "Mark Hardwick"
                       },
                       new Coach
                       {
                           CoachName = "Dave Kendell"
                       },
                       new Coach
                       {
                           CoachName = "David Stovell"
                       },
                       new Coach
                       {
                           CoachName = "Joshua Kaluba"
                       }, 
                       new Coach
                       {
                           CoachName = "Melisa Vanderley"
                       });
                    context.SaveChanges();
                }

                if (!context.Locations.Any())
                {
                    context.Locations.AddRange(
                         new Location
                         {
                             LocationName = "Stadium A",

                         },
                        new Location
                        {
                            LocationName = "Field B"
                        },
                        new Location
                        {
                            LocationName = "Stadium C",
                        },
                        new Location
                        {
                            LocationName = "Field D",
                        },
                        new Location
                        {
                            LocationName = "Stadium E",
                        }
                        );
                }
                if (!context.Outcomes.Any())
                {
                    context.Outcomes.AddRange(
                         new Outcome
                         {
                             OutcomeString = "TBD",

                         },
                        new Outcome
                        {
                            OutcomeString = "Win-Home"
                        },
                        new Outcome
                        {
                            OutcomeString = "Win-Away",
                        },
                        new Outcome
                        {
                            OutcomeString = "Tie"
                        }
                        );
                }

                //Adding Division
                if (!context.Divisions.Any())
                {
                    context.Divisions.AddRange(
                        new Division
                        {
                            DivisionName = "9U",
                            ClubID = context.Clubs.FirstOrDefault(c => c.ClubName == "Welland Minor Baseball Association").ID
                        },
                        new Division
                        {
                            DivisionName = "11U",
                            ClubID = context.Clubs.FirstOrDefault(c => c.ClubName == "Welland Minor Baseball Association").ID
                        },
                        new Division
                        {
                            DivisionName = "13U",
                            ClubID = context.Clubs.FirstOrDefault(c => c.ClubName == "Welland Minor Baseball Association").ID
                        },
                        new Division
                        {
                            DivisionName = "15U",
                            ClubID = context.Clubs.FirstOrDefault(c => c.ClubName == "Welland Minor Baseball Association").ID
                        },
                        new Division
                        {
                            DivisionName = "18U",
                            ClubID = context.Clubs.FirstOrDefault(c => c.ClubName == "Welland Minor Baseball Association").ID
                        });
                    context.SaveChanges();
                }
                

                //To add team
                if (!context.Teams.Any())
                {
                    context.Teams.AddRange(
                        new Team
                        {
                            TeamName = "Bananas",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Mark Hardwick").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID
                        },
                        new Team
                        {
                            TeamName = "Iron Birds",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "David Stovell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID
                        },
                        new Team
                        {
                            TeamName = "Whitecaps",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Mark Hardwick").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID
                        },
                        new Team
                        {
                            TeamName = "Bisons",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID
                        },
                        new Team
                        {
                            TeamName = "Dragons",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID
                        },
                        new Team
                        {
                            TeamName = "Trash Pandas",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "David Stovell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID
                        });
                    context.SaveChanges();
                }

                

                
                
                //if (!context.TeamGames.Any())
                //{
                //    context.TeamGames.AddRange(
                //         new TeamGame
                //         {
                //             HomeTeamID = 1,
                //             AwayTeamID = 2,
                //             GameID = 1
                //         },
                //        new TeamGame
                //        {
                //            HomeTeamID = 1,
                //            AwayTeamID = 3,
                //            GameID = 2,
                //        },
                //        new TeamGame
                //        {
                //            HomeTeamID = 2,
                //            AwayTeamID = 1,
                //            GameID = 3,
                //        },
                //        new TeamGame
                //        {
                //            HomeTeamID = 2,
                //            AwayTeamID = 3,
                //            GameID = 4,
                //        },
                //        new TeamGame
                //        {
                //            HomeTeamID = 4,
                //            AwayTeamID = 2,
                //            GameID = 5,
                //        }
                //        );
                //}

                //Adding Players
                if (!context.Players.Any())
                {
                    context.Players.AddRange(
                        new Player
                        {
                            FirstName = "Lance",
                            LastName = "Glaus",
                            MemberID = "FE9113FF",
                            JerseyNumber = 12,
                            TeamID  = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Brad",
                            LastName = "Lyon",
                            MemberID = "2C5E0779",
                            JerseyNumber = 11,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Steve",
                            Nickname = "Goat",
                            LastName = "Harrington",
                            MemberID = "2J5Y0779",
                            JerseyNumber = 22,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Dustin",
                            Nickname = "Speedy",
                            LastName = "Henderson",
                            MemberID = "2F5H0659",
                            JerseyNumber = 6,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Alphonso",
                            Nickname = "Phonzy",
                            LastName = "Davies",
                            MemberID = "2B5B6779",
                            JerseyNumber = 5,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Satoru",
                            Nickname = "Capitan",
                            LastName = "Endo",
                            MemberID = "2D5H3279",
                            JerseyNumber = 1,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Brandon",
                            Nickname = "Tuki",
                            LastName = "Villegas",
                            MemberID = "2F5F0779",
                            JerseyNumber = 9,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Lionel",
                            Nickname = "Leo",
                            LastName = "Messi",
                            MemberID = "2M3M0779",
                            JerseyNumber = 10,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Cristiano",
                            Nickname = "CR7",
                            LastName = "Ronaldo",
                            MemberID = "2CR70779",
                            JerseyNumber = 7,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Lucas",
                            LastName = "Sinclair",
                            MemberID = "2D5D8879",
                            JerseyNumber = 23,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Daniel",
                            Nickname = "Karate Kid",
                            LastName = "Larusso",
                            MemberID = "02dc3bfa",
                            JerseyNumber = 2,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Jose",
                            LastName = "Clark",
                            MemberID = "02fc3bfa",
                            JerseyNumber = 1,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Diego",
                            Nickname = "Piggy",
                            LastName = "Alas",
                            MemberID = "12mc3bfa",
                            JerseyNumber = 3,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Gamaliel",
                            LastName = "Romualdo",
                            MemberID = "13hc3dfa",
                            JerseyNumber = 4,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Naruto",
                            LastName = "Uzumaki",
                            MemberID = "12nt3chk",
                            JerseyNumber = 11,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Fernando",
                            LastName = "Alonso",
                            MemberID = "103c3bfa",
                            JerseyNumber = 10,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Bernardo",
                            LastName = "Silva",
                            MemberID = "14fg3bfa",
                            JerseyNumber = 11,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Erling",
                            Nickname = "Majin buu",
                            LastName = "Halland",
                            MemberID = "16hj3bfa",
                            JerseyNumber = 9,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Jose",
                            LastName = "Santos",
                            MemberID = "12kh3bfa",
                            JerseyNumber = 13,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Carl",
                            Nickname = "CJ",
                            LastName = "Johnson",
                            MemberID = "12cj3big",
                            JerseyNumber = 66,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Royce",
                            LastName = "Fil",
                            MemberID = "d60fd621",
                            JerseyNumber = 2,

                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Injured").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                        },
                        new Player
                        {
                            FirstName = "Julio",
                            LastName = "Glaus",
                            MemberID = "df2db445",
                            JerseyNumber = 9,

                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Brian",
                            LastName = "Counsell",
                            MemberID = "6148791e",
                            JerseyNumber = 8,

                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                        },
                        new Player
                        {
                            FirstName = "Adam",
                            LastName = "Estes",
                            MemberID = "67476a1e",
                            JerseyNumber = 7,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        },
                        new Player
                        {
                            FirstName = "Mike",
                            LastName = "Hill",
                            MemberID = "F2C6F254",
                            JerseyNumber = 5,
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID,
                            StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                        }
                    );

                    //Adding more players with random values, so we have more players in more Teams
                    string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla" };
                    string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker"};
                    int[] teamIDs = context.Teams.Select(d => d.ID).ToArray();
                    int teamIDCount = teamIDs.Length;
                    //Loop through names and add more
                    foreach (string lastName in lastNames)
                    {
                        foreach (string firstname in firstNames)
                        {
                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(11111111,99999999).ToString(),
                                JerseyNumber = random.Next(0,99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = teamIDs[random.Next(teamIDCount)],
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                            };
                            context.Players.Add(a);
                        }
                    }
                    context.SaveChanges();
                    context.SaveChanges();
                }//End adding the players for Bananas and IronBirds (Hardcoded and random numbers)

                
                //Adding Games
                if (!context.Games.Any())
                {

                    context.Games.AddRange(
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-02-01 14:00:00"),
                             HomeTeamID = 1,
                             AwayTeamID = 2,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 3
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-04-07 14:00:00"),
                             HomeTeamID = 4,
                             AwayTeamID = 5,
                             LocationID = 3,
                             OutcomeID = 1,
                             DivisionID = 4
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-04-06 14:00:00"),
                             HomeTeamID = 5,
                             AwayTeamID = 6,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 4
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-06 14:00:00"),
                             HomeTeamID = 1,
                             AwayTeamID = 3,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 3
                         }, new Game
                         {
                             StartTime = DateTime.Parse("2024-03-04 14:00:00"),
                             HomeTeamID = 2,
                             AwayTeamID = 3,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 3
                         });
                    context.SaveChanges();


                    //Create initial lineups to match team membership
                    foreach (Game game in context.Games)
                    {

                        //Add the players from the teams to each one
                        Team homeTeam = context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.HomeTeamID);
                        foreach (Player p in homeTeam.Players)
                        {
                            game.GamePlayers.Add(new GamePlayer()
                            {
                                PlayerID = p.ID,
                                GameID = game.ID,
                                TeamLineup = TeamLineup.Home
                            });
                        }

                        Team awayTeam = context.Teams.Include(t => t.Players).FirstOrDefault(t => t.ID == game.AwayTeamID);
                        foreach (Player p in awayTeam.Players)
                        {
                            game.GamePlayers.Add(new GamePlayer()
                            {
                                PlayerID = p.ID,
                                GameID = game.ID,
                                TeamLineup = TeamLineup.Away
                            });
                        }
                        context.SaveChanges();
                    }
                }//end seed data for games
                if (!context.Innings.Any())
                {
                    for (int i = 1; i <= 7; i++)
                    {
                        context.Innings.Add(new Inning
                        {
                            InningNo = $"Inning {i}",
                            GameID = 1
                        });
                        context.Innings.Add(new Inning
                        {
                            InningNo = $"Inning {i}",
                            GameID = 2
                        });
                        context.Innings.Add(new Inning
                        {
                            InningNo = $"Inning {i}",
                            GameID = 3
                        });
                        context.Innings.Add(new Inning
                        {
                            InningNo = $"Inning {i}",
                            GameID = 4
                        });
                        context.Innings.Add(new Inning
                        {
                            InningNo = $"Inning {i}",
                            GameID = 5
                        });
                    }

                    context.SaveChanges();
                }
                //Adding Stats
                if (!context.Stats.Any())
                {
                    Random rnd = new Random();


                    context.Stats.Add(new Stat
                    {
                        GamesPlayed = rnd.Next(20, 60),
                        PlayerAppearance = rnd.Next(20, 50),
                        Hits = rnd.Next(10, 30),
                        RunsScored = rnd.Next(5, 20),
                        StrikeOuts = rnd.Next(5, 25),
                        Walks = rnd.Next(2, 15),
                        RBI = rnd.Next(5, 15),
                        PlayerID = 1,
                    });

                    context.Stats.Add(new Stat
                    {
                        GamesPlayed = rnd.Next(20, 60),
                        PlayerAppearance = rnd.Next(20, 50),
                        Hits = rnd.Next(10, 30),
                        RunsScored = rnd.Next(5, 20),
                        StrikeOuts = rnd.Next(5, 25),
                        Walks = rnd.Next(2, 15),
                        RBI = rnd.Next(5, 15),
                        PlayerID = 2,
                    });

                    context.SaveChanges();
                }
                if (!context.Scores.Any())
                {
                    Random rnd = new Random();

                    // Seed scores for PlayerID = 1
                    for (int i = 1; i <= 7; i++)
                    {
                        context.Scores.Add(new Score
                        {
                            Balls = rnd.Next(0, 4), // Balls range from 0 to 3
                            FoulBalls = rnd.Next(0, 3), // Foul balls range from 0 to 2
                            Strikes = rnd.Next(0, 3), // Strikes range from 0 to 2
                            Out = rnd.Next(0, 3), // Outs range from 0 to 2
                            Runs = rnd.Next(0, 5), // Runs range from 0 to 4
                            Hits = rnd.Next(0, 5), // Hits range from 0 to 4
                            PlayerID = 1,
                            InningID = i,
                            GameID = 1
                        });
                    }

                    // Seed scores for PlayerID = 2
                    //for (int i = 8; i <= 14; i++)
                    //{
                    //    context.Scores.Add(new Score
                    //    {
                    //        Balls = rnd.Next(0, 4),
                    //        FoulBalls = rnd.Next(0, 3),
                    //        Strikes = rnd.Next(0, 3),
                    //        Out = rnd.Next(0, 3),
                    //        Runs = rnd.Next(0, 5),
                    //        Hits = rnd.Next(0, 5),
                    //        PlayerID = 2,
                    //        InningID = i,
                    //        GameID = 1
                    //    });
                    //}

                    context.SaveChanges();
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}

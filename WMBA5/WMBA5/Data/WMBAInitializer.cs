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
                context.Database.EnsureCreated();

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
            

                if (!context.Games.Any())
                {
                    context.Games.AddRange(
                         new Game
                         {
                             ID = 1,
                             StartTime = DateTime.Parse("2024-02-01 14:00:00"),
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 1
                         },
                        new Game
                        {
                            ID = 2,
                            StartTime = DateTime.Parse("2024-02-05 15:30:00"),
                            LocationID = 2,
                            OutcomeID = 1,
                            DivisionID = 2
                        },
                        new Game
                        {
                            ID = 3,
                            StartTime = DateTime.Parse("2024-02-10 13:45:00"),
                            LocationID = 3,
                            OutcomeID = 1,
                            DivisionID = 1
                        },
                        new Game
                        {
                            ID = 4,
                            StartTime = DateTime.Parse("2024-02-15 16:00:00"),
                            LocationID = 1,
                            OutcomeID = 1,
                            DivisionID = 2
                        },
                        new Game
                        {
                            ID = 5,
                            StartTime = DateTime.Parse("2024-02-20 14:15:00"),
                            LocationID = 4,
                            OutcomeID = 1,
                            DivisionID = 1
                        }
                        );
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
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID,
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
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID,
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
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                        });
                    context.SaveChanges();
                }
                //Adding PlayerStats
                if (!context.PlayerStats.Any())
                {
                    context.PlayerStats.AddRange(
                        new PlayerStat
                        {
                            ID = 1,
                            GamesPlayed = 50,
                            PlayerAppearance = 45,
                            Hits = 25,
                            RunsScored = 10,
                            StrikeOuts = 15,
                            Walks = 5,
                            RBI = 8,
                            PlayerID = 1, // Assuming PlayerID corresponds to an existing Player's ID
                          GameID = 3
                        },
                       new PlayerStat
                       {
                           ID = 2,
                           GamesPlayed = 45,
                           PlayerAppearance = 40,
                           Hits = 20,
                           RunsScored = 12,
                           StrikeOuts = 10,
                           Walks = 8,
                           RBI = 10,
                           PlayerID = 1, // Assuming PlayerID corresponds to an existing Player's ID
                          GameID = 2
                       },
                       new PlayerStat
                       {
                           ID = 3,
                           GamesPlayed = 55,
                           PlayerAppearance = 50,
                           Hits = 30,
                           RunsScored = 15,
                           StrikeOuts = 12,
                           Walks = 7,
                           RBI = 12,
                           PlayerID = 1, // Assuming PlayerID corresponds to an existing Player's ID
                         GameID = 1
                       },
                        new PlayerStat
                        {
                            ID = 4,
                            GamesPlayed = 48,
                            PlayerAppearance = 42,
                            Hits = 22,
                            RunsScored = 11,
                            StrikeOuts = 14,
                            Walks = 6,
                            RBI = 9,
                            PlayerID = 2, // Assuming PlayerID corresponds to an existing Player's ID
                              GameID = 1
                        },
                        new PlayerStat
                        {
                            ID = 5,
                            GamesPlayed = 60,
                            PlayerAppearance = 55,
                            Hits = 28,
                            RunsScored = 13,
                            StrikeOuts = 18,
                            Walks = 8,
                            RBI = 11,
                            PlayerID = 2, // Assuming PlayerID corresponds to an existing Player's ID
                              GameID =2
                        },
                       new PlayerStat
                       {
                           ID = 6,
                           GamesPlayed = 42,
                           PlayerAppearance = 38,
                           Hits = 18,
                           RunsScored = 9,
                           StrikeOuts = 11,
                           Walks = 5,
                           RBI = 7,
                           PlayerID = 2, // Assuming PlayerID corresponds to an existing Player's ID
                          GameID = 3
                       },
                        new PlayerStat
                        {
                            ID = 7,
                            GamesPlayed = 47,
                            PlayerAppearance = 43,
                            Hits = 23,
                            RunsScored = 12,
                            StrikeOuts = 13,
                            Walks = 7,
                            RBI = 10,
                            PlayerID = 2, // Assuming PlayerID corresponds to an existing Player's ID
                            GameID = 4
                        },
                        new PlayerStat
                        {
                            ID = 8,
                            GamesPlayed = 52,
                            PlayerAppearance = 48,
                            Hits = 27,
                            RunsScored = 14,
                            StrikeOuts = 16,
                            Walks = 6,
                            RBI = 13,
                            PlayerID = 3, // Assuming PlayerID corresponds to an existing Player's ID
                            GameID = 1
                        });
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

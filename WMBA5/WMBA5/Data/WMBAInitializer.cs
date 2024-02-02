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
                           CoachName = "Dave Kendall"
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
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Mark Hardwick").ID,
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
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendall").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID
                        },
                        new Team
                        {
                            TeamName = "Dragons",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendall").ID,
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

                //Adding Players
                if (!context.Players.Any())
                {
                    context.Players.AddRange(
                        new Player
                        {
                            FirstName = "Lance",
                            LastName = "Glaus",
                            MemberID = "FE9113FF",
                            JerseyNumber = 10,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID  = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID
                        },
                        new Player
                        {
                            FirstName = "Brad",
                            LastName = "Lyon",
                            MemberID = "2C5E0779",
                            JerseyNumber = 11,
                            Birthday = DateTime.Parse("2010-02-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Bananas").ID
                        },
                        new Player
                        {
                            FirstName = "Jose",
                            LastName = "Clark",
                            MemberID = "02fc3bfa",
                            JerseyNumber = 1,
                            Birthday = DateTime.Parse("2010-03-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID
                        },
                        new Player
                        {
                            FirstName = "Royce",
                            LastName = "Fil",
                            MemberID = "d60fd621",
                            JerseyNumber = 2,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Iron Birds").ID
                        },
                        new Player
                        {
                            FirstName = "Julio",
                            LastName = "Glaus",
                            MemberID = "df2db445",
                            JerseyNumber = 9,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID
                        },
                        new Player
                        {
                            FirstName = "Brian",
                            LastName = "Counsell",
                            MemberID = "6148791e",
                            JerseyNumber = 8,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID
                        },
                        new Player
                        {
                            FirstName = "Adam",
                            LastName = "Estes",
                            MemberID = "67476a1e",
                            JerseyNumber = 7,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID
                        },
                        new Player
                        {
                            FirstName = "Mike",
                            LastName = "Hill",
                            MemberID = "F2C6F254",
                            JerseyNumber = 5,
                            Birthday = DateTime.Parse("2010-01-01"),
                            Position = "Bats",
                            TeamID = context.Teams.FirstOrDefault(c => c.TeamName == "Whitecaps").ID
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
                            PlayerApperance = 45,
                            Hits = 25,
                            RunsScored = 10,
                            StrikeOuts = 15,
                            Walks = 5,
                            RBI = 8,
                            PlayerID = 1 // Assuming PlayerID corresponds to an existing Player's ID
                        },
                       new PlayerStat
                       {
                           ID = 2,
                           GamesPlayed = 45,
                           PlayerApperance = 40,
                           Hits = 20,
                           RunsScored = 12,
                           StrikeOuts = 10,
                           Walks = 8,
                           RBI = 10,
                           PlayerID = 1 // Assuming PlayerID corresponds to an existing Player's ID
                       },
                       new PlayerStat
                       {
                           ID = 3,
                           GamesPlayed = 55,
                           PlayerApperance = 50,
                           Hits = 30,
                           RunsScored = 15,
                           StrikeOuts = 12,
                           Walks = 7,
                           RBI = 12,
                           PlayerID = 1 // Assuming PlayerID corresponds to an existing Player's ID
                       },
                        new PlayerStat
                        {
                            ID = 4,
                            GamesPlayed = 48,
                            PlayerApperance = 42,
                            Hits = 22,
                            RunsScored = 11,
                            StrikeOuts = 14,
                            Walks = 6,
                            RBI = 9,
                            PlayerID = 2 // Assuming PlayerID corresponds to an existing Player's ID
                        },
                        new PlayerStat
                        {
                            ID = 5,
                            GamesPlayed = 60,
                            PlayerApperance = 55,
                            Hits = 28,
                            RunsScored = 13,
                            StrikeOuts = 18,
                            Walks = 8,
                            RBI = 11,
                            PlayerID = 2 // Assuming PlayerID corresponds to an existing Player's ID
                        },
                       new PlayerStat
                       {
                           ID = 6,
                           GamesPlayed = 42,
                           PlayerApperance = 38,
                           Hits = 18,
                           RunsScored = 9,
                           StrikeOuts = 11,
                           Walks = 5,
                           RBI = 7,
                           PlayerID = 2 // Assuming PlayerID corresponds to an existing Player's ID
                       },
                        new PlayerStat
                        {
                            ID = 7,
                            GamesPlayed = 47,
                            PlayerApperance = 43,
                            Hits = 23,
                            RunsScored = 12,
                            StrikeOuts = 13,
                            Walks = 7,
                            RBI = 10,
                            PlayerID = 2 // Assuming PlayerID corresponds to an existing Player's ID
                        },
                        new PlayerStat
                        {
                            ID = 8,
                            GamesPlayed = 52,
                            PlayerApperance = 48,
                            Hits = 27,
                            RunsScored = 14,
                            StrikeOuts = 16,
                            Walks = 6,
                            RBI = 13,
                            PlayerID = 2 // Assuming PlayerID corresponds to an existing Player's ID
                        });
                    context.SaveChanges();
                }
                if (!context.Games.Any())
                {
                    context.Games.AddRange(
                        new Game
                        {
                            StartTime = DateTime.Parse("2024-03-05"),
                            Location = "Field 1",
                            Oponent = "Iron Birds",
                            PlayingAt = "Home",
                            Outcome = "Yet to be Decided",
                            DivisionID = context.Divisions.FirstOrDefault(d => d.DivisionName == "13U").ID
                        },
                        new Game
                        {
                            StartTime = DateTime.Parse("2024-03-10"),
                            Location = "Field 2",
                            Oponent = "Whitecaps",
                            PlayingAt = "Away",
                            Outcome = "Yet to be Decided",
                            DivisionID = context.Divisions.FirstOrDefault(d => d.DivisionName == "13U").ID
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

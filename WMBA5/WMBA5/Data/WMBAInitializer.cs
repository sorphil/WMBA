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
                           CoachName = "Melisa Vanderlely"
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
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID
                        },
                        //U15
                        new Team
                        {
                            TeamName = "Bisons",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Mark Hardwick").ID,
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
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Joshua Kaluba").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID
                        },
                        //Adding more teams following feedback
                        //U9
                        new Team
                        {
                            TeamName = "Raimon",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "David Stovell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID
                        },
                        new Team
                        {
                            TeamName = "Orfeo",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID
                        },
                        new Team
                        {
                            TeamName = "Kings",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Mark Hardwick").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID
                        },
                        //U11
                        new Team
                        {
                            TeamName = "Trash Pandas",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "David Stovell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID
                        },
                        new Team
                        {
                            TeamName = "Angry Birds",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID
                        },
                        new Team
                        {
                            TeamName = "G2",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Melisa Vanderlely").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID
                        },
                        //U18
                        new Team
                        {
                            TeamName = "Raimon",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Joshua Kaluba").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID
                        },
                        new Team
                        {
                            TeamName = "Leviathan",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Dave Kendell").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID
                        },
                        new Team
                        {
                            TeamName = "Wellanders",
                            CoachID = context.Coaches.FirstOrDefault(c => c.CoachName == "Melisa Vanderlely").ID,
                            DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID
                        }
                        );
                    context.SaveChanges();
                }

                #region Hardcoded Players
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
                    #endregion
                    #region Array of Names
                    //Adding more players with random values, so we have more players in more Teams
                    string[] firstNames = new string[] { "Lyric", "Antoinette", "Vivian" };
                    string[] lastNames = new string[] { "Watts", "Randall", "Arias", };
                    int[] teamIDs = context.Teams.Select(d => d.ID).ToArray();
                    int teamIDCount = teamIDs.Length;
                    string[] firstNames2 = new string[] { "Ruth", "Jamison", "Emilia", "Natalee" };
                    string[] lastNames2 = new string[] { "Weber", "Stone", "Carlson" };
                    string[] firstNames3 = new string[] { "Yadiel", "Jakayla", "Lukas", "Moses" };
                    string[] lastNames3= new string[] { "Robles", "Frederick", "Parker" };
                    string[] firstNames4 = new string[] { "Chris", "Tommas", "David", "Sam" };
                    string[] lastNames4 = new string[] { "Bumstead", "Mazza", "Laid" };
                    //New names for more teams
                    string[] firstNames5 = new string[] { "Juan", "Jorge", "Gama", "Joshua" };
                    string[] lastNames5= new string[] { "Weber", "Wagner", "Carlson" };
                    string[] firstNames6 = new string[] { "Rishi", "Philip", "Subit", "Cole" };
                    string[] lastNames6 = new string[] { "Warner", "Soriano", "Paudyal" };
                    string[] firstNames7 = new string[] { "Donnis", "Apolo", "Rocky", "Sam" };
                    string[] lastNames7 = new string[] { "Creed", "Balboa", "Marquez" };
                    string[] firstNames8 = new string[] { "Ruth", "Jamison", "Emilia", "Natalee" };
                    string[] lastNames8 = new string[] { "Weber", "Stone", "Carlson" };
                    string[] firstNames9 = new string[] { "Yadiel", "Jakayla", "Lukas", "Moses" };
                    string[] lastNames9 = new string[] { "Robles", "Frederick", "Parker" };
                    string[] firstNames10 = new string[] { "Chris", "Tommas", "David", "Sam" };
                    string[] lastNames10 = new string[] { "Bumstead", "Mazza", "Laid" };
                    #endregion
                    //Loop through names and add more
                    //For whitecaps team
                    foreach (string lastName in lastNames)
                        {
                            foreach (string firstname in firstNames)
                            {

                                //Construct some details
                                Player a = new Player()
                                {
                                    FirstName = firstname,
                                    LastName = lastName,
                                    MemberID = random.Next(11111111, 24500000).ToString(),
                                    JerseyNumber = random.Next(0, 99),
                                    StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                    TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Whitecaps").ID,
                                    DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "13U").ID,
                                };
                                context.Players.Add(a);


                            }
                        }
                    #region U15 players
                    //U15
                    //For Bisons
                    foreach (string lastName in lastNames2)
                    {
                        foreach (string firstname in firstNames2)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(24500001, 37500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Bisons").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    //For Dragons
                    foreach (string lastName in lastNames3)
                    {
                        foreach (string firstname in firstNames3)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(37500001, 44500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Dragons").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    //For Trash Pandas
                    foreach (string lastName in lastNames4)
                    {
                        foreach (string firstname in firstNames4)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(44500001, 57500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Trash Pandas").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "15U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    #endregion
                    #region 9U Players
                    //U9
                    //For Raimon
                    foreach (string lastName in lastNames5)
                    {
                        foreach (string firstname in firstNames5)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(57500000, 59500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Raimon" && t.Division.DivisionName =="9U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For Orfeo
                    foreach (string lastName in lastNames6)
                    {
                        foreach (string firstname in firstNames6)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(59500001, 63500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Orfeo" && t.Division.DivisionName == "9U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For Kings
                    foreach (string lastName in lastNames5)
                    {
                        foreach (string firstname in firstNames5)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(63500001, 67500000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Kings" && t.Division.DivisionName == "9U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "9U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    #endregion
                    #region 11U Players
                    //U11
                    //For Trash Pandas
                    foreach (string lastName in lastNames7)
                    {
                        foreach (string firstname in firstNames7)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(67500001, 70000000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Trash Pandas" && t.Division.DivisionName == "11U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For Angry Birds
                    foreach (string lastName in lastNames8)
                    {
                        foreach (string firstname in firstNames8)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(70000001, 70025000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Angry Birds" && t.Division.DivisionName == "11U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For G2
                    foreach (string lastName in lastNames9)
                    {
                        foreach (string firstname in firstNames9)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(70025001, 70055001).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "G2" && t.Division.DivisionName == "11U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "11U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    #endregion
                    #region 18U Players
                    //U18
                    //For Raimon
                    foreach (string lastName in lastNames10)
                    {
                        foreach (string firstname in firstNames10)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(70055002, 70800000).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Raimon" && t.Division.DivisionName == "18U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For Leviathan
                    foreach (string lastName in lastNames8)
                    {
                        foreach (string firstname in firstNames)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(70800001, 80002500).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Leviathan" && t.Division.DivisionName == "18U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    context.SaveChanges();
                    //For Wellanders
                    foreach (string lastName in lastNames4)
                    {
                        foreach (string firstname in firstNames3)
                        {

                            //Construct some details
                            Player a = new Player()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MemberID = random.Next(80002501, 80040500).ToString(),
                                JerseyNumber = random.Next(0, 99),
                                StatusID = context.Statuses.FirstOrDefault(c => c.StatusName == "Active").ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.TeamName == "Wellanders" && t.Division.DivisionName == "18U").ID,
                                DivisionID = context.Divisions.FirstOrDefault(c => c.DivisionName == "18U").ID,
                            };
                            context.Players.Add(a);


                        }
                    }
                    #endregion
                    context.SaveChanges();


                    //context.SaveChanges();
                }//End adding the players for all Teams (Hardcoded and random numbers)


                //Adding Games
                if (!context.Games.Any())
                {

                    context.Games.AddRange(
                    #region 13U Games
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-06-01 14:00:00"),
                             HomeTeamID = 1,
                             AwayTeamID = 2,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 3
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-06-02 14:00:00"),
                             HomeTeamID = 1,
                             AwayTeamID = 3,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 3
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-06-03 14:00:00"),
                             HomeTeamID = 2,
                             AwayTeamID = 3,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 3
                         },
                    #endregion
                    #region 15U Games
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
                              StartTime = DateTime.Parse("2024-04-05 14:00:00"),
                              HomeTeamID = 6,
                              AwayTeamID = 4,
                              LocationID = 4,
                              OutcomeID = 1,
                              DivisionID = 4
                          },
                    #endregion
                    #region 11U Games
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-06 14:00:00"),
                             HomeTeamID = 10,
                             AwayTeamID = 11,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 2
                         }, 
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-07-04 14:00:00"),
                             HomeTeamID = 11,
                             AwayTeamID = 12,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 2
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-08 14:00:00"),
                             HomeTeamID = 12,
                             AwayTeamID = 10,
                             LocationID = 4,
                             OutcomeID = 1,
                             DivisionID = 2
                         },
                    #endregion
                    #region 9U Games
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-09 10:00:00"),
                             HomeTeamID = 7,
                             AwayTeamID = 8,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 1
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-07-07 15:00:00"),
                             HomeTeamID = 8,
                             AwayTeamID = 9,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 1
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-08 13:00:00"),
                             HomeTeamID = 9,
                             AwayTeamID = 7,
                             LocationID = 3,
                             OutcomeID = 1,
                             DivisionID = 1
                         },
                    #endregion
                    #region 18U Games
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-10 10:00:00"),
                             HomeTeamID = 13,
                             AwayTeamID = 14,
                             LocationID = 2,
                             OutcomeID = 1,
                             DivisionID = 5
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-07-01 15:00:00"),
                             HomeTeamID = 15,
                             AwayTeamID = 13,
                             LocationID = 1,
                             OutcomeID = 1,
                             DivisionID = 5
                         },
                         new Game
                         {
                             StartTime = DateTime.Parse("2024-05-11 13:00:00"),
                             HomeTeamID = 14,
                             AwayTeamID = 15,
                             LocationID = 3,
                             OutcomeID = 1,
                             DivisionID = 5
                         }
                    #endregion
                         );
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

                //Adding Stats
                //if (!context.Stats.Any())
                //{
                //    Random rnd = new Random();


                //    context.Stats.Add(new Stat
                //    {
                //        GamesPlayed = rnd.Next(20, 60),
                //        PlayerAppearance = rnd.Next(20, 50),
                //        Hits = rnd.Next(10, 30),
                //        RunsScored = rnd.Next(5, 20),
                //        StrikeOuts = rnd.Next(5, 25),
                //        Walks = rnd.Next(2, 15),
                //        RBI = rnd.Next(5, 15),
                //        PlayerID = 1,
                //    });

                //    context.Stats.Add(new Stat
                //    {
                //        GamesPlayed = rnd.Next(20, 60),
                //        PlayerAppearance = rnd.Next(20, 50),
                //        Hits = rnd.Next(10, 30),
                //        RunsScored = rnd.Next(5, 20),
                //        StrikeOuts = rnd.Next(5, 25),
                //        Walks = rnd.Next(2, 15),
                //        RBI = rnd.Next(5, 15),
                //        PlayerID = 2,
                //    });

                //    context.SaveChanges();
                //}
                //if (!context.Scores.Any())
                //{
                //    Random rnd = new Random();

                //    // Seed scores for PlayerID = 1
                //    for (int i = 1; i <= 7; i++)
                //    {
                //        context.Scores.Add(new Score
                //        {
                //            Balls = rnd.Next(0, 4), // Balls range from 0 to 3
                //            FoulBalls = rnd.Next(0, 3), // Foul balls range from 0 to 2
                //            Strikes = rnd.Next(0, 3), // Strikes range from 0 to 2
                //            Out = rnd.Next(0, 3), // Outs range from 0 to 2
                //            Runs = rnd.Next(0, 5), // Runs range from 0 to 4
                //            Hits = rnd.Next(0, 5), // Hits range from 0 to 4
                //            PlayerID = 1,
                //            InningID = i,
                //            GameID = 1
                //        });
                //    }

                //    // Seed scores for PlayerID = 2
                //    //for (int i = 8; i <= 14; i++)
                //    //{
                //    //    context.Scores.Add(new Score
                //    //    {
                //    //        Balls = rnd.Next(0, 4),
                //    //        FoulBalls = rnd.Next(0, 3),
                //    //        Strikes = rnd.Next(0, 3),
                //    //        Out = rnd.Next(0, 3),
                //    //        Runs = rnd.Next(0, 5),
                //    //        Hits = rnd.Next(0, 5),
                //    //        PlayerID = 2,
                //    //        InningID = i,
                //    //        GameID = 1
                //    //    });
                //    //}

                //    context.SaveChanges();
                //}
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}

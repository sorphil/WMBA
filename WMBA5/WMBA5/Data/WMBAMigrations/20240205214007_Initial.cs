using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMBA5.Data.WMBAMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClubName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoachName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatusName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DivisionName = table.Column<string>(type: "TEXT", nullable: false),
                    ClubID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Divisions_Clubs_ClubID",
                        column: x => x.ClubID,
                        principalTable: "Clubs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Outcome = table.Column<string>(type: "TEXT", nullable: true),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Games_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamName = table.Column<string>(type: "TEXT", nullable: false),
                    CoachID = table.Column<int>(type: "INTEGER", nullable: false),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_Coaches_CoachID",
                        column: x => x.CoachID,
                        principalTable: "Coaches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teams_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Innings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InningNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    RunsScored = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayersOut = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Innings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Innings_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberID = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    JerseyNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Players_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Statuses_StatusID",
                        column: x => x.StatusID,
                        principalTable: "Statuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HomeTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamGame_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamGame_Teams_AwayTeamID",
                        column: x => x.AwayTeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamGame_Teams_HomeTeamID",
                        column: x => x.HomeTeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAtBats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<string>(type: "TEXT", nullable: true),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    InningID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAtBats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerAtBats_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerAtBats_Innings_InningID",
                        column: x => x.InningID,
                        principalTable: "Innings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerAtBats_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GamesPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerAppearance = table.Column<int>(type: "INTEGER", nullable: false),
                    Hits = table.Column<int>(type: "INTEGER", nullable: false),
                    RunsScored = table.Column<int>(type: "INTEGER", nullable: false),
                    StrikeOuts = table.Column<int>(type: "INTEGER", nullable: false),
                    Walks = table.Column<int>(type: "INTEGER", nullable: false),
                    RBI = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_ClubID",
                table: "Divisions",
                column: "ClubID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_DivisionID",
                table: "Games",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Innings_GameID",
                table: "Innings",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAtBats_GameID",
                table: "PlayerAtBats",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAtBats_InningID",
                table: "PlayerAtBats",
                column: "InningID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAtBats_PlayerID",
                table: "PlayerAtBats",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_DivisionID",
                table: "Players",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_MemberID",
                table: "Players",
                column: "MemberID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_StatusID",
                table: "Players",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_GameID",
                table: "PlayerStats",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_PlayerID",
                table: "PlayerStats",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGame_AwayTeamID",
                table: "TeamGame",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGame_GameID",
                table: "TeamGame",
                column: "GameID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGame_HomeTeamID",
                table: "TeamGame",
                column: "HomeTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CoachID",
                table: "Teams",
                column: "CoachID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_DivisionID",
                table: "Teams",
                column: "DivisionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAtBats");

            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "TeamGame");

            migrationBuilder.DropTable(
                name: "Innings");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Clubs");
        }
    }
}

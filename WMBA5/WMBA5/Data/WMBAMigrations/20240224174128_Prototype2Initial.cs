using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMBA5.Data.WMBAMigrations
{
    /// <inheritdoc />
    public partial class Prototype2Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBats_Games_GameID",
                table: "PlayerAtBats");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBats_Innings_InningID",
                table: "PlayerAtBats");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBats_Players_PlayerID",
                table: "PlayerAtBats");

            migrationBuilder.DropTable(
                name: "TeamGame");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAtBats",
                table: "PlayerAtBats");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "PlayerAtBats",
                newName: "PlayerAtBat");

            migrationBuilder.RenameColumn(
                name: "RunsScored",
                table: "Innings",
                newName: "Strikes");

            migrationBuilder.RenameColumn(
                name: "PlayersOut",
                table: "Innings",
                newName: "Runs");

            migrationBuilder.RenameColumn(
                name: "InningNumber",
                table: "Innings",
                newName: "Out");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBats_PlayerID",
                table: "PlayerAtBat",
                newName: "IX_PlayerAtBat_PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBats_InningID",
                table: "PlayerAtBat",
                newName: "IX_PlayerAtBat_InningID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBats_GameID",
                table: "PlayerAtBat",
                newName: "IX_PlayerAtBat_GameID");

            migrationBuilder.AddColumn<int>(
                name: "Balls",
                table: "Innings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoulBalls",
                table: "Innings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hits",
                table: "Innings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayTeamID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeTeamID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OutcomeID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAtBat",
                table: "PlayerAtBat",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "GamePlayers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamLineup = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameScores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Inning = table.Column<int>(type: "INTEGER", nullable: false),
                    Balls = table.Column<int>(type: "INTEGER", nullable: false),
                    FoulBalls = table.Column<int>(type: "INTEGER", nullable: false),
                    Strikes = table.Column<int>(type: "INTEGER", nullable: false),
                    Out = table.Column<int>(type: "INTEGER", nullable: false),
                    Runs = table.Column<int>(type: "INTEGER", nullable: false),
                    Hits = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerAtBatID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GameScores_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScores_PlayerAtBat_PlayerAtBatID",
                        column: x => x.PlayerAtBatID,
                        principalTable: "PlayerAtBat",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InGameStats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Balls = table.Column<int>(type: "INTEGER", nullable: false),
                    Strikes = table.Column<int>(type: "INTEGER", nullable: false),
                    Out = table.Column<int>(type: "INTEGER", nullable: false),
                    PlateApperance = table.Column<int>(type: "INTEGER", nullable: false),
                    Runs = table.Column<int>(type: "INTEGER", nullable: false),
                    RBI = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InGameStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InGameStats_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InGameStats_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lineup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lineup_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lineup_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Outcomes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OutcomeString = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameScores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameScoreID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerGameScores_GameScores_GameScoreID",
                        column: x => x.GameScoreID,
                        principalTable: "GameScores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerGameScores_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLineups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    LineupID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLineups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerLineups_Lineup_LineupID",
                        column: x => x.LineupID,
                        principalTable: "Lineup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerLineups_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamID",
                table: "Games",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamID",
                table: "Games",
                column: "HomeTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LocationID",
                table: "Games",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OutcomeID",
                table: "Games",
                column: "OutcomeID");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_GameID",
                table: "GamePlayers",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_PlayerID_GameID",
                table: "GamePlayers",
                columns: new[] { "PlayerID", "GameID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_GameID",
                table: "GameScores",
                column: "GameID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_PlayerAtBatID",
                table: "GameScores",
                column: "PlayerAtBatID");

            migrationBuilder.CreateIndex(
                name: "IX_InGameStats_GameID",
                table: "InGameStats",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_InGameStats_PlayerID",
                table: "InGameStats",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Lineup_GameID",
                table: "Lineup",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_Lineup_TeamID",
                table: "Lineup",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameScores_GameScoreID",
                table: "PlayerGameScores",
                column: "GameScoreID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameScores_PlayerID",
                table: "PlayerGameScores",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLineups_LineupID",
                table: "PlayerLineups",
                column: "LineupID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLineups_PlayerID",
                table: "PlayerLineups",
                column: "PlayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Locations_LocationID",
                table: "Games",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Outcomes_OutcomeID",
                table: "Games",
                column: "OutcomeID",
                principalTable: "Outcomes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Teams_AwayTeamID",
                table: "Games",
                column: "AwayTeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Teams_HomeTeamID",
                table: "Games",
                column: "HomeTeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBat_Games_GameID",
                table: "PlayerAtBat",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBat_Innings_InningID",
                table: "PlayerAtBat",
                column: "InningID",
                principalTable: "Innings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBat_Players_PlayerID",
                table: "PlayerAtBat",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Locations_LocationID",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Outcomes_OutcomeID",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Teams_AwayTeamID",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Teams_HomeTeamID",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBat_Games_GameID",
                table: "PlayerAtBat");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBat_Innings_InningID",
                table: "PlayerAtBat");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAtBat_Players_PlayerID",
                table: "PlayerAtBat");

            migrationBuilder.DropTable(
                name: "GamePlayers");

            migrationBuilder.DropTable(
                name: "InGameStats");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Outcomes");

            migrationBuilder.DropTable(
                name: "PlayerGameScores");

            migrationBuilder.DropTable(
                name: "PlayerLineups");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "GameScores");

            migrationBuilder.DropTable(
                name: "Lineup");

            migrationBuilder.DropIndex(
                name: "IX_Games_AwayTeamID",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_HomeTeamID",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_LocationID",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_OutcomeID",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAtBat",
                table: "PlayerAtBat");

            migrationBuilder.DropColumn(
                name: "Balls",
                table: "Innings");

            migrationBuilder.DropColumn(
                name: "FoulBalls",
                table: "Innings");

            migrationBuilder.DropColumn(
                name: "Hits",
                table: "Innings");

            migrationBuilder.DropColumn(
                name: "AwayTeamID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeTeamID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "OutcomeID",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "PlayerAtBat",
                newName: "PlayerAtBats");

            migrationBuilder.RenameColumn(
                name: "Strikes",
                table: "Innings",
                newName: "RunsScored");

            migrationBuilder.RenameColumn(
                name: "Runs",
                table: "Innings",
                newName: "PlayersOut");

            migrationBuilder.RenameColumn(
                name: "Out",
                table: "Innings",
                newName: "InningNumber");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBat_PlayerID",
                table: "PlayerAtBats",
                newName: "IX_PlayerAtBats_PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBat_InningID",
                table: "PlayerAtBats",
                newName: "IX_PlayerAtBats_InningID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerAtBat_GameID",
                table: "PlayerAtBats",
                newName: "IX_PlayerAtBats_GameID");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Games",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAtBats",
                table: "PlayerAtBats",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "TeamGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AwayTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamID = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBats_Games_GameID",
                table: "PlayerAtBats",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBats_Innings_InningID",
                table: "PlayerAtBats",
                column: "InningID",
                principalTable: "Innings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAtBats_Players_PlayerID",
                table: "PlayerAtBats",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

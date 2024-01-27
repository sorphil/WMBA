using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMBA5.Data.WMBAMigrations
{
    /// <inheritdoc />
    public partial class AfterApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineupID",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineupID",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineupID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                name: "PlayerStats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GamesPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerApperance = table.Column<int>(type: "INTEGER", nullable: false),
                    Hits = table.Column<int>(type: "INTEGER", nullable: false),
                    RunsScored = table.Column<int>(type: "INTEGER", nullable: false),
                    StrikeOuts = table.Column<int>(type: "INTEGER", nullable: false),
                    Walks = table.Column<int>(type: "INTEGER", nullable: false),
                    RBI = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lineups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsPlaying = table.Column<bool>(type: "INTEGER", nullable: false),
                    BattingOrder = table.Column<string>(type: "TEXT", nullable: true),
                    FieldingPosition = table.Column<string>(type: "TEXT", nullable: true),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lineups_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lineups_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lineups_Teams_TeamID",
                        column: x => x.TeamID,
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
                name: "IX_PlayerStats_PlayerID",
                table: "PlayerStats",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Lineups_GameID",
                table: "Lineups",
                column: "GameID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineups_PlayerID",
                table: "Lineups",
                column: "PlayerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineups_TeamID",
                table: "Lineups",
                column: "TeamID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAtBats");

            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "Lineups");

            migrationBuilder.DropTable(
                name: "Innings");

            migrationBuilder.DropColumn(
                name: "LineupID",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LineupID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LineupID",
                table: "Games");
        }
    }
}

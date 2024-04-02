using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMBA5.Data.WMBAMigrations
{
    /// <inheritdoc />
    public partial class AddedCurrentInning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentInningID",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentInningID1",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentInningID1",
                table: "Games",
                column: "CurrentInningID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Innings_CurrentInningID1",
                table: "Games",
                column: "CurrentInningID1",
                principalTable: "Innings",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Innings_CurrentInningID1",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_CurrentInningID1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CurrentInningID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CurrentInningID1",
                table: "Games");
        }
    }
}

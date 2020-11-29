using Microsoft.EntityFrameworkCore.Migrations;

namespace Tabula.Migrations
{
    public partial class SetOnDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pins_Boards_BoardId",
                table: "Pins");

            migrationBuilder.AddForeignKey(
                name: "FK_Pins_Boards_BoardId",
                table: "Pins",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pins_Boards_BoardId",
                table: "Pins");

            migrationBuilder.AddForeignKey(
                name: "FK_Pins_Boards_BoardId",
                table: "Pins",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

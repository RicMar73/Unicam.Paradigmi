using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unicam.Paradigmi.Models.Migrations
{
    /// <inheritdoc />
    public partial class CorrezioneBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Resources_ResourceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Bookings",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Bookings",
                newName: "End");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ResourceId_StartDate_EndDate",
                table: "Bookings",
                newName: "IX_Bookings_ResourceId_Start_End");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Bookings",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Bookings",
                newName: "EndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ResourceId_Start_End",
                table: "Bookings",
                newName: "IX_Bookings_ResourceId_StartDate_EndDate");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Resources_ResourceId",
                table: "Bookings",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

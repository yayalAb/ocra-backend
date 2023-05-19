using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class databaseUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvents_Addresses_BirthPlaceId",
                table: "BirthEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvents_Lookups_BirthPlaceId",
                table: "BirthEvents",
                column: "BirthPlaceId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvents_Lookups_BirthPlaceId",
                table: "BirthEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvents_Addresses_BirthPlaceId",
                table: "BirthEvents",
                column: "BirthPlaceId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

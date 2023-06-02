using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class restrictondelete1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events",
                column: "EventAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events",
                column: "EventAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

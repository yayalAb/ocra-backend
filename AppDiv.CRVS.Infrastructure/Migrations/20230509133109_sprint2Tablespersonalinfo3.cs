using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class sprint2Tablespersonalinfo3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}

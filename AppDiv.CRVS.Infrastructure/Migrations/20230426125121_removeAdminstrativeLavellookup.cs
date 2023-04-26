using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class removeAdminstrativeLavellookup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Lookups_AdminLevelLookupId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_AdminLevelLookupId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AdminLevelLookupId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminLevel",
                table: "Addresses");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminLevelLookupId",
                table: "Addresses",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AdminLevelLookupId",
                table: "Addresses",
                column: "AdminLevelLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Lookups_AdminLevelLookupId",
                table: "Addresses",
                column: "AdminLevelLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

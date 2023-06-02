using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DuringDeath",
                table: "DeathEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "DuringDeathId",
                table: "DeathEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DeathEvents_DuringDeathId",
                table: "DeathEvents",
                column: "DuringDeathId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvents_Lookups_DuringDeathId",
                table: "DeathEvents",
                column: "DuringDeathId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvents_Lookups_DuringDeathId",
                table: "DeathEvents");

            migrationBuilder.DropIndex(
                name: "IX_DeathEvents_DuringDeathId",
                table: "DeathEvents");

            migrationBuilder.DropColumn(
                name: "DuringDeathId",
                table: "DeathEvents");

            migrationBuilder.AddColumn<string>(
                name: "DuringDeath",
                table: "DeathEvents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

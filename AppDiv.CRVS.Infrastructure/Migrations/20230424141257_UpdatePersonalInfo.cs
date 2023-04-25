using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class UpdatePersonalInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactInfo_PersonalInfos_PersonalInfoId",
                table: "ContactInfo");

            migrationBuilder.DropIndex(
                name: "IX_ContactInfo_PersonalInfoId",
                table: "ContactInfo");

            migrationBuilder.DropColumn(
                name: "PersonalInfoId",
                table: "ContactInfo");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactInfoId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInfos_ContactInfoId",
                table: "PersonalInfos",
                column: "ContactInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos",
                column: "ContactInfoId",
                principalTable: "ContactInfo",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos");

            migrationBuilder.DropIndex(
                name: "IX_PersonalInfos_ContactInfoId",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "ContactInfoId",
                table: "PersonalInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonalInfoId",
                table: "ContactInfo",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfo_PersonalInfoId",
                table: "ContactInfo",
                column: "PersonalInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactInfo_PersonalInfos_PersonalInfoId",
                table: "ContactInfo",
                column: "PersonalInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

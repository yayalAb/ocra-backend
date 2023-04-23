using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class parentAddressIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Addresses_ParentAddressId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId1",
                table: "PersonalInfos");

            migrationBuilder.DropIndex(
                name: "IX_PersonalInfos_TitleLookupId1",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "TitleLookupId1",
                table: "PersonalInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentAddressId",
                table: "Addresses",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInfos_TypeOfWorkLookupId",
                table: "PersonalInfos",
                column: "TypeOfWorkLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Addresses_ParentAddressId",
                table: "Addresses",
                column: "ParentAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos",
                column: "TypeOfWorkLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Addresses_ParentAddressId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropIndex(
                name: "IX_PersonalInfos_TypeOfWorkLookupId",
                table: "PersonalInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "TitleLookupId1",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentAddressId",
                table: "Addresses",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInfos_TitleLookupId1",
                table: "PersonalInfos",
                column: "TitleLookupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Addresses_ParentAddressId",
                table: "Addresses",
                column: "ParentAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId1",
                table: "PersonalInfos",
                column: "TitleLookupId1",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

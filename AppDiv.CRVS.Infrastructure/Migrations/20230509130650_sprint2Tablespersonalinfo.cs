using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class sprint2Tablespersonalinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Addresses_AddressId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_NationalityLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_SexLookupId",
                table: "PersonalInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "PersonalInfos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNameStr",
                table: "PersonalInfos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "MarriageStatusLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactInfoId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Addresses_AddressId",
                table: "PersonalInfos",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos",
                column: "ContactInfoId",
                principalTable: "ContactInfo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_NationalityLookupId",
                table: "PersonalInfos",
                column: "NationalityLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_SexLookupId",
                table: "PersonalInfos",
                column: "SexLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Addresses_AddressId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_NationalityLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_SexLookupId",
                table: "PersonalInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "PersonalInfos",
                keyColumn: "NationalId",
                keyValue: null,
                column: "NationalId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "PersonalInfos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "PersonalInfos",
                keyColumn: "MiddleNameStr",
                keyValue: null,
                column: "MiddleNameStr",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNameStr",
                table: "PersonalInfos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "MarriageStatusLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactInfoId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Addresses_AddressId",
                table: "PersonalInfos",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_ContactInfo_ContactInfoId",
                table: "PersonalInfos",
                column: "ContactInfoId",
                principalTable: "ContactInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_NationalityLookupId",
                table: "PersonalInfos",
                column: "NationalityLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_SexLookupId",
                table: "PersonalInfos",
                column: "SexLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

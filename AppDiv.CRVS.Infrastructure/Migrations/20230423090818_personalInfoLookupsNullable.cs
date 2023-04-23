using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class personalInfoLookupsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_EducationalStatusLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_MarriageStatusLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_NationLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_PlaceOfBirthLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_ReligionLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeOfWorkLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "TitleLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReligionLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlaceOfBirthLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "EducationalStatusLookupId",
                table: "PersonalInfos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_EducationalStatusLookupId",
                table: "PersonalInfos",
                column: "EducationalStatusLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_MarriageStatusLookupId",
                table: "PersonalInfos",
                column: "MarriageStatusLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_NationLookupId",
                table: "PersonalInfos",
                column: "NationLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_PlaceOfBirthLookupId",
                table: "PersonalInfos",
                column: "PlaceOfBirthLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_ReligionLookupId",
                table: "PersonalInfos",
                column: "ReligionLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId",
                table: "PersonalInfos",
                column: "TitleLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos",
                column: "TypeOfWorkLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_EducationalStatusLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_MarriageStatusLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_NationLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_PlaceOfBirthLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_ReligionLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeOfWorkLookupId",
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
                name: "TitleLookupId",
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
                name: "ReligionLookupId",
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
                name: "PlaceOfBirthLookupId",
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
                name: "NationLookupId",
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
                name: "EducationalStatusLookupId",
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
                name: "FK_PersonalInfos_Lookups_EducationalStatusLookupId",
                table: "PersonalInfos",
                column: "EducationalStatusLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_MarriageStatusLookupId",
                table: "PersonalInfos",
                column: "MarriageStatusLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_NationLookupId",
                table: "PersonalInfos",
                column: "NationLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_PlaceOfBirthLookupId",
                table: "PersonalInfos",
                column: "PlaceOfBirthLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_ReligionLookupId",
                table: "PersonalInfos",
                column: "ReligionLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TitleLookupId",
                table: "PersonalInfos",
                column: "TitleLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Lookups_TypeOfWorkLookupId",
                table: "PersonalInfos",
                column: "TypeOfWorkLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

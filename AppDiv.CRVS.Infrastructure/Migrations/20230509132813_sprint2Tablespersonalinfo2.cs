using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class sprint2Tablespersonalinfo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtCaseId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdoptiveMotherId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdoptiveFatherId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents",
                column: "CourtCaseId",
                principalTable: "CourtCases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents",
                column: "AdoptiveFatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents",
                column: "AdoptiveMotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtCaseId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdoptiveMotherId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdoptiveFatherId",
                table: "AdoptionEvents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents",
                column: "CourtCaseId",
                principalTable: "CourtCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents",
                column: "AdoptiveFatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents",
                column: "AdoptiveMotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

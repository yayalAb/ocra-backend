using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class exepmtionSupportingDocRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "SupportingDocuments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentExamptionId",
                table: "SupportingDocuments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SupportingDocuments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SupportingDocuments_PaymentExamptionId",
                table: "SupportingDocuments",
                column: "PaymentExamptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportingDocuments_PaymentExamptions_PaymentExamptionId",
                table: "SupportingDocuments",
                column: "PaymentExamptionId",
                principalTable: "PaymentExamptions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportingDocuments_PaymentExamptions_PaymentExamptionId",
                table: "SupportingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_SupportingDocuments_PaymentExamptionId",
                table: "SupportingDocuments");

            migrationBuilder.DropColumn(
                name: "PaymentExamptionId",
                table: "SupportingDocuments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SupportingDocuments");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "SupportingDocuments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

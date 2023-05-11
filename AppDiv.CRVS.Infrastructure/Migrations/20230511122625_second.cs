using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamptionRequests_PaymentExamptions_ExamptionRequestN~",
                table: "PaymentExamptionRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentExamptionRequests_ExamptionRequestNavigationId",
                table: "PaymentExamptionRequests");

            migrationBuilder.DropColumn(
                name: "ExamptionRequestNavigationId",
                table: "PaymentExamptionRequests");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentExamptions_ExamptionRequestId",
                table: "PaymentExamptions",
                column: "ExamptionRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamptions_PaymentExamptionRequests_ExamptionRequestId",
                table: "PaymentExamptions",
                column: "ExamptionRequestId",
                principalTable: "PaymentExamptionRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamptions_PaymentExamptionRequests_ExamptionRequestId",
                table: "PaymentExamptions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentExamptions_ExamptionRequestId",
                table: "PaymentExamptions");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamptionRequestNavigationId",
                table: "PaymentExamptionRequests",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentExamptionRequests_ExamptionRequestNavigationId",
                table: "PaymentExamptionRequests",
                column: "ExamptionRequestNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamptionRequests_PaymentExamptions_ExamptionRequestN~",
                table: "PaymentExamptionRequests",
                column: "ExamptionRequestNavigationId",
                principalTable: "PaymentExamptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

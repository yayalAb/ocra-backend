using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class updateWorkflowStepMOdel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibleGroup",
                table: "Steps");

            migrationBuilder.RenameColumn(
                name: "DescreptionStr",
                table: "Workflows",
                newName: "DescriptionStr");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Steps",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Payment",
                table: "Steps",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "StepUserGroup",
                columns: table => new
                {
                    StepsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserGroupsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepUserGroup", x => new { x.StepsId, x.UserGroupsId });
                    table.ForeignKey(
                        name: "FK_StepUserGroup_Steps_StepsId",
                        column: x => x.StepsId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StepUserGroup_UserGroups_UserGroupsId",
                        column: x => x.UserGroupsId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StepUserGroup_UserGroupsId",
                table: "StepUserGroup",
                column: "UserGroupsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepUserGroup");

            migrationBuilder.RenameColumn(
                name: "DescriptionStr",
                table: "Workflows",
                newName: "DescreptionStr");

            migrationBuilder.AlterColumn<float>(
                name: "Status",
                table: "Steps",
                type: "float",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<float>(
                name: "Payment",
                table: "Steps",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleGroup",
                table: "Steps",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

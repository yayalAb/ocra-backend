using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class datatypeUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("0b322a93-e9ce-4075-89a3-b201c9494788"));

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("0dca3d1c-8976-4922-91da-23f69ac0ec9c"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("8f92dba4-26a5-4d69-aada-62c75e12717c"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("e62c8a5e-16c1-4343-8644-f4f661a18ff7"));

            migrationBuilder.DropColumn(
                name: "AuditUser",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Suffixes",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Suffixes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Genders",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Genders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Customers",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Customers",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Suffixes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Genders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuditUserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f39722d5-9b37-42ad-8186-acc7a34afbb7", "AQAAAAEAACcQAAAAEJGvaCaie2Iod0uQXjmOKRhDpnaM2U7e82HrpD7adUnpHHoRg0RYVUwhvYns1GJmTg==", "95f6e2c2-1d61-4879-8269-baac7f5cc80b" });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("a52b334a-b2ce-4445-b3a3-c83fd9ccd859"), "F", new DateTime(2023, 3, 13, 17, 34, 34, 439, DateTimeKind.Local).AddTicks(1444), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Female" },
                    { new Guid("ed8220f8-006d-4869-9c5c-2a3c889a5ef5"), "M", new DateTime(2023, 3, 13, 17, 34, 34, 439, DateTimeKind.Local).AddTicks(1442), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Male" }
                });

            migrationBuilder.InsertData(
                table: "Suffixes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("1d7a21ba-7941-4648-ad67-abd6ed6d0151"), new DateTime(2023, 3, 13, 17, 34, 34, 439, DateTimeKind.Local).AddTicks(1459), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mr." },
                    { new Guid("5446b04a-4289-46ad-a7e4-2805c5f5d566"), new DateTime(2023, 3, 13, 17, 34, 34, 439, DateTimeKind.Local).AddTicks(1462), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mrs." }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("a52b334a-b2ce-4445-b3a3-c83fd9ccd859"));

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("ed8220f8-006d-4869-9c5c-2a3c889a5ef5"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("1d7a21ba-7941-4648-ad67-abd6ed6d0151"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("5446b04a-4289-46ad-a7e4-2805c5f5d566"));

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Suffixes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Genders");

            migrationBuilder.DropColumn(
                name: "AuditUserId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Suffixes",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Suffixes",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Genders",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Genders",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Customers",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Customers",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<string>(
                name: "AuditUser",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "46d329bc-16e2-4475-b670-f05c41a7c8ea", "AQAAAAEAACcQAAAAEE6LhKAG03SNUjX3v6YpsGjCbmhU3JRCzepBW3b+L2IlOJPjVEvI5AJ0jkhY4avLfw==", "20fe60ef-25d1-4600-8b9b-a7c777ac570a" });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Code", "CreatedDate", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("0b322a93-e9ce-4075-89a3-b201c9494788"), "F", new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8202), null, new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8200), "Female" },
                    { new Guid("0dca3d1c-8976-4922-91da-23f69ac0ec9c"), "M", new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8199), null, new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8173), "Male" }
                });

            migrationBuilder.InsertData(
                table: "Suffixes",
                columns: new[] { "Id", "CreatedDate", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("8f92dba4-26a5-4d69-aada-62c75e12717c"), new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8244), null, new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8232), "Mrs." },
                    { new Guid("e62c8a5e-16c1-4343-8644-f4f661a18ff7"), new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8230), null, new DateTime(2023, 3, 13, 15, 45, 13, 919, DateTimeKind.Local).AddTicks(8228), "Mr." }
                });
        }
    }
}

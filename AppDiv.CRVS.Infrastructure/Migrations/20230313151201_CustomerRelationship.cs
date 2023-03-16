using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class CustomerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Genders_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Suffixes_Id",
                table: "Customers");

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

            migrationBuilder.AddColumn<Guid>(
                name: "GenderId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SuffixId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0abc490-f980-4e3d-b6f7-45cb1fbcd27e", "AQAAAAEAACcQAAAAEGxXQgRLpTIDXjL9ppOgWJQQ8mN6Z6yzLaNiDD/hLhRgJJeKWPoQvAJU6D8K/eo6bA==", "4eec67b7-29da-4d0b-8d94-153f82977049" });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("3544ccfd-648c-46f8-87c9-93ccf6a5b017"), "F", new DateTime(2023, 3, 13, 18, 12, 1, 358, DateTimeKind.Local).AddTicks(237), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Female" },
                    { new Guid("99abce36-b63c-4a2b-a123-6621092143aa"), "M", new DateTime(2023, 3, 13, 18, 12, 1, 358, DateTimeKind.Local).AddTicks(235), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Male" }
                });

            migrationBuilder.InsertData(
                table: "Suffixes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("66f5b62a-55b6-4781-920f-48695a91eed8"), new DateTime(2023, 3, 13, 18, 12, 1, 358, DateTimeKind.Local).AddTicks(262), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mr." },
                    { new Guid("affa9d07-aa49-4025-960a-57e647accbb7"), new DateTime(2023, 3, 13, 18, 12, 1, 358, DateTimeKind.Local).AddTicks(265), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mrs." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_GenderId",
                table: "Customers",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_SuffixId",
                table: "Customers",
                column: "SuffixId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Genders_GenderId",
                table: "Customers",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Suffixes_SuffixId",
                table: "Customers",
                column: "SuffixId",
                principalTable: "Suffixes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Genders_GenderId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Suffixes_SuffixId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_GenderId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_SuffixId",
                table: "Customers");

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("3544ccfd-648c-46f8-87c9-93ccf6a5b017"));

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("99abce36-b63c-4a2b-a123-6621092143aa"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("66f5b62a-55b6-4781-920f-48695a91eed8"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("affa9d07-aa49-4025-960a-57e647accbb7"));

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SuffixId",
                table: "Customers");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Genders_Id",
                table: "Customers",
                column: "Id",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Suffixes_Id",
                table: "Customers",
                column: "Id",
                principalTable: "Suffixes",
                principalColumn: "Id");
        }
    }
}

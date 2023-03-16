using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class updateCustomerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Genders_Id",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Suffixes_Id",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("6f8be52e-a3ec-4806-a419-3dab971b41a7"));

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: new Guid("c9505df7-ba61-43fe-b0a9-f0be891a3605"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("57025bbd-d151-4f12-953a-9f9be8e48b91"));

            migrationBuilder.DeleteData(
                table: "Suffixes",
                keyColumn: "Id",
                keyValue: new Guid("f45627a3-14a4-4138-a31a-1474a52b1944"));

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Genders_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Suffixes_Id",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

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

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8cbf7ce1-c6ee-464e-b5f6-e41e937b2c34", "AQAAAAEAACcQAAAAEGAPSByI0CHyZ+mYkyAu+QwDy315p9UlQxYPSyhLCs1qN2Rn9k3kjVQKNZbUakuYrg==", "c929e98f-726e-42be-9831-6ec077a1f24c" });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Code", "CreatedDate", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("6f8be52e-a3ec-4806-a419-3dab971b41a7"), "M", new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7787), null, new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7773), "Male" },
                    { new Guid("c9505df7-ba61-43fe-b0a9-f0be891a3605"), "F", new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7789), null, new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7788), "Female" }
                });

            migrationBuilder.InsertData(
                table: "Suffixes",
                columns: new[] { "Id", "CreatedDate", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("57025bbd-d151-4f12-953a-9f9be8e48b91"), new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7807), null, new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7805), "Mr." },
                    { new Guid("f45627a3-14a4-4138-a31a-1474a52b1944"), new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7808), null, new DateTime(2023, 3, 13, 15, 39, 7, 89, DateTimeKind.Local).AddTicks(7807), "Mrs." }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Genders_Id",
                table: "Customer",
                column: "Id",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Suffixes_Id",
                table: "Customer",
                column: "Id",
                principalTable: "Suffixes",
                principalColumn: "Id");
        }
    }
}

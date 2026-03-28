using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingGearBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddProductLogistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOrderOnly",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreOrder",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreOrderDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsOrderOnly", "IsPreOrder", "PreOrderDate" },
                values: new object[] { false, false, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsOrderOnly", "IsPreOrder", "PreOrderDate", "Stock" },
                values: new object[] { false, false, null, 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsOrderOnly", "IsPreOrder", "PreOrderDate" },
                values: new object[] { false, true, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsOrderOnly", "IsPreOrder", "PreOrderDate" },
                values: new object[] { false, false, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsOrderOnly", "IsPreOrder", "PreOrderDate", "Stock" },
                values: new object[] { true, false, null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrderOnly",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPreOrder",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PreOrderDate",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Stock",
                value: 30);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Stock",
                value: 60);
        }
    }
}

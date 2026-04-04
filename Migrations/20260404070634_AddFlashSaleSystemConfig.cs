using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamingGearBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashSaleSystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FlashSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsHidden",
                value: false);

            migrationBuilder.InsertData(
                table: "SystemConfigs",
                columns: new[] { "Id", "Description", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "Phí vận chuyển mặc định (VNĐ)", "shipping_fee", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "30000" },
                    { 2, "Đơn hàng từ giá trị này được miễn phí ship (VNĐ)", "free_shipping_threshold", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "500000" },
                    { 3, "Số ngày được phép đổi trả hàng", "return_policy_days", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "7" },
                    { 4, "Điều kiện đổi trả hàng", "return_policy_note", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sản phẩm cần còn nguyên tem, hộp và chưa qua sử dụng" },
                    { 5, "Tên hiển thị của website", "site_name", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SCYTOL CLX21" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashSales");

            migrationBuilder.DropTable(
                name: "SystemConfigs");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Products");
        }
    }
}

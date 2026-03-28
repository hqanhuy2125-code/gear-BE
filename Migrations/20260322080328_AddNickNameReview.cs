using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingGearBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddNickNameReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NickName",
                table: "Reviews");
        }
    }
}

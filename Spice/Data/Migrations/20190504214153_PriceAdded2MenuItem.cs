using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class PriceAdded2MenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Spicness",
                table: "MenuItem",
                newName: "Spycness");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "MenuItem",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "MenuItem");

            migrationBuilder.RenameColumn(
                name: "Spycness",
                table: "MenuItem",
                newName: "Spicness");
        }
    }
}

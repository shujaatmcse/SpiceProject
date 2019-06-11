using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class ShoppingCartModelMenuItemUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_shoppingCart_MenuItemId",
                table: "shoppingCart",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCart_MenuItem_MenuItemId",
                table: "shoppingCart",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCart_MenuItem_MenuItemId",
                table: "shoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_shoppingCart_MenuItemId",
                table: "shoppingCart");
        }
    }
}

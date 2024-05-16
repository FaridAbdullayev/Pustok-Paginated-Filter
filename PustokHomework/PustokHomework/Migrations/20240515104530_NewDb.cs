using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokHomework.Migrations
{
    /// <inheritdoc />
    public partial class NewDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Basketss_AspNetUsers_AppUserId",
                table: "Basketss");

            migrationBuilder.DropForeignKey(
                name: "FK_Basketss_Books_BookId",
                table: "Basketss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Basketss",
                table: "Basketss");

            migrationBuilder.RenameTable(
                name: "Basketss",
                newName: "Baskets");

            migrationBuilder.RenameIndex(
                name: "IX_Basketss_BookId",
                table: "Baskets",
                newName: "IX_Baskets_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Basketss_AppUserId",
                table: "Baskets",
                newName: "IX_Baskets_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_AspNetUsers_AppUserId",
                table: "Baskets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Books_BookId",
                table: "Baskets",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_AspNetUsers_AppUserId",
                table: "Baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Books_BookId",
                table: "Baskets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets");

            migrationBuilder.RenameTable(
                name: "Baskets",
                newName: "Basketss");

            migrationBuilder.RenameIndex(
                name: "IX_Baskets_BookId",
                table: "Basketss",
                newName: "IX_Basketss_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Baskets_AppUserId",
                table: "Basketss",
                newName: "IX_Basketss_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Basketss",
                table: "Basketss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Basketss_AspNetUsers_AppUserId",
                table: "Basketss",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Basketss_Books_BookId",
                table: "Basketss",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

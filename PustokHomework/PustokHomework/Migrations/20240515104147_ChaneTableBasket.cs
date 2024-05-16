using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokHomework.Migrations
{
    /// <inheritdoc />
    public partial class ChaneTableBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Baskets");

            migrationBuilder.RenameTable(
                name: "Baskets",
                newName: "Basketss");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Basketss",
                newName: "BookId");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Basketss",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Basketss",
                table: "Basketss",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Basketss_AppUserId",
                table: "Basketss",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Basketss_BookId",
                table: "Basketss",
                column: "BookId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Basketss_AppUserId",
                table: "Basketss");

            migrationBuilder.DropIndex(
                name: "IX_Basketss_BookId",
                table: "Basketss");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Basketss");

            migrationBuilder.RenameTable(
                name: "Basketss",
                newName: "Baskets");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Baskets",
                newName: "ProductId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Baskets",
                table: "Baskets",
                column: "Id");
        }
    }
}

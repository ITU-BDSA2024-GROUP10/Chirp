using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLikesToAuthorAndCheep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_AuthorId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Cheeps_CheepId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "Like");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_CheepId",
                table: "Like",
                newName: "IX_Like_CheepId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_AuthorId",
                table: "Like",
                newName: "IX_Like_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_AspNetUsers_AuthorId",
                table: "Like",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Cheeps_CheepId",
                table: "Like",
                column: "CheepId",
                principalTable: "Cheeps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_AspNetUsers_AuthorId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Cheeps_CheepId",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameIndex(
                name: "IX_Like_CheepId",
                table: "Likes",
                newName: "IX_Likes_CheepId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_AuthorId",
                table: "Likes",
                newName: "IX_Likes_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_AuthorId",
                table: "Likes",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Cheeps_CheepId",
                table: "Likes",
                column: "CheepId",
                principalTable: "Cheeps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleDB.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingAttributeNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Cheeps",
                newName: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Cheeps",
                newName: "Text");
        }
    }
}

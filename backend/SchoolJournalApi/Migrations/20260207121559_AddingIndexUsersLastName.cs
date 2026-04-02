using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexUsersLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_LastName",
                table: "Users");
        }
    }
}

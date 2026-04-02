using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class IsActiveClassAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentClasses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentClasses");
        }
    }
}

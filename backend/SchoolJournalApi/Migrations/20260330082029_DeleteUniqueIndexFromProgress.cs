using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUniqueIndexFromProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Progresses_UserId_LessonId",
                table: "Progresses");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_UserId",
                table: "Progresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Progresses_UserId",
                table: "Progresses");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_UserId_LessonId",
                table: "Progresses",
                columns: new[] { "UserId", "LessonId" },
                unique: true);
        }
    }
}

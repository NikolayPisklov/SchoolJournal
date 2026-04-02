using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddingUniqueKeyToJournalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Journals_ClassId",
                table: "Journals");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_ClassId_TeacherSubjectId",
                table: "Journals",
                columns: new[] { "ClassId", "TeacherSubjectId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Journals_ClassId_TeacherSubjectId",
                table: "Journals");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_ClassId",
                table: "Journals",
                column: "ClassId");
        }
    }
}

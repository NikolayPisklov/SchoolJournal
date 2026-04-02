using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_UserId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_UserId",
                table: "StudentClasses");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_UserId_SubjectId",
                table: "TeacherSubjects",
                columns: new[] { "UserId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_UserId_ClassId",
                table: "StudentClasses",
                columns: new[] { "UserId", "ClassId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_UserId_SubjectId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_UserId_ClassId",
                table: "StudentClasses");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_UserId",
                table: "TeacherSubjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_UserId",
                table: "StudentClasses",
                column: "UserId");
        }
    }
}

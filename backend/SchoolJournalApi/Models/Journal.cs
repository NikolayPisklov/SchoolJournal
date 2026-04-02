using Microsoft.EntityFrameworkCore.Query.Internal;

namespace SchoolJournalApi.Models
{
    public class Journal
    {
        public int Id { get; set; }
        public int TeacherSubjectId { get; set; }
        public int ClassId { get; set; }
        public int Year { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public TeacherSubject TeacherSubject { get; set; } = null!;
        public Class Class { get; set; } = null!;

    }
}

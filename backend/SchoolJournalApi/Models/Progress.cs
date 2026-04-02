namespace SchoolJournalApi.Models
{
    public class Progress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public int? MarkId { get; set; }
        public int AttendanceId { get; set; }
        public DateTime ProgressUpdateDate { get; set; }
        public bool IsUpdated { get; set; }

        public User Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
        public Mark? Mark { get; set; } 
        public Attendance Attendance { get; set; } = null!;
    }
}

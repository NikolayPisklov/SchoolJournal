namespace SchoolJournalApi.Dtos.Progress
{
    public class JournalProgressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public int? MarkValue { get; set; }
        public string AttendanceValue { get; set; } = string.Empty;
        public DateTime ProgressUpdateTime { get; set; }
    }
}

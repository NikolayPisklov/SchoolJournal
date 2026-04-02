namespace SchoolJournalApi.Dtos.Lesson
{
    public class LessonDto
    {
        public int Id { get; set; }
        public int JournalId { get; set; }
        public DateOnly LessonDate { get; set; }
        public string Theme { get; set; } = string.Empty;
        public string Homework { get; set; } = string.Empty;
    }
}

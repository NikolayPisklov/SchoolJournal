namespace SchoolJournalApi.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int JournalId { get; set; }
        public DateOnly LessonDate {  get; set; }
        public string Theme { get; set; } = string.Empty;
        public string Homework { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public Journal Journal { get; set; } = null!;
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}

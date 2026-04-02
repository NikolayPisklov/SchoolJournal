namespace SchoolJournalApi.Models
{
    public class TeacherSubject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }

        public ICollection<Journal> Journals { get; set; } = new List<Journal>();
        public User Teacher { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}

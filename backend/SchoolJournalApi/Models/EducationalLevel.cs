namespace SchoolJournalApi.Models
{
    public class EducationalLevel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}

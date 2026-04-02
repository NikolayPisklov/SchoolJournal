namespace SchoolJournalApi.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int EducationalLevelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }

        public EducationalLevel EducationalLevel { get; set; } = null!;
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<Journal> Journals { get; set; } = new List<Journal>();
    }
}

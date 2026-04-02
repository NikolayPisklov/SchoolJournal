namespace SchoolJournalApi.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public int EducationalLevelId { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public EducationalLevel EducationalLevel { get; set; } = null!;
    }
}

namespace SchoolJournalApi.Dtos.Journal
{
    public class JournalInListDto
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int TeacherSubjectId { get; set; }
        public string SubjectTitle { get; set; } = string.Empty;
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherMiddleName { get; set; } = string.Empty;
        public string ClassTitle { get; set; } = string.Empty;  
        public int ClassYear { get; set; }
        public int JournalYear { get; set; }
    }
}

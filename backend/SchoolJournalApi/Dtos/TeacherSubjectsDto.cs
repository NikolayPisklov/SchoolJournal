namespace SchoolJournalApi.Dto_s
{
    public class TeacherSubjectsDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public int SubjectEducationalLevelId { get; set; }
        public string SubjectTitle { get; set; } = string.Empty;
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherMiddleName { get; set; } = string.Empty;
    }
}

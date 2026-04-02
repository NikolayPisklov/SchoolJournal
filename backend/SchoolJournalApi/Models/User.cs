namespace SchoolJournalApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public string Login {  get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;

        public Status Status { get; set; } = null!;
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    }
}

namespace SchoolJournalApi.Dtos.User
{
    public class ListedStudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }  = string.Empty;
        public string LastName { get; set; }  = string.Empty;
        public string MiddleName { get; set; }  = string.Empty;
    }
}

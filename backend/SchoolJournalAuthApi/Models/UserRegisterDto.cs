namespace SchoolJournalAuthApi.Models
{
    public class UserRegisterDto
    {
        public int StatusId { get; set; }

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string MiddleName { get; set; } = null!;
    }
}

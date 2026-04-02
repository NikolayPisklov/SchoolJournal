namespace SchoolJournalApi.Dtos.User
{
    public class ListedUserDto
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string? ClassTitle { get; set; }
        public int? ClassYear { get; set; }
    }
}

namespace SchoolJournalApi.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

namespace SchoolJournalApi.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;

        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}

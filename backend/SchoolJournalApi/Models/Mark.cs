namespace SchoolJournalApi.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public List<Progress> Progresses { get; set; } = new List<Progress>();
    }
}

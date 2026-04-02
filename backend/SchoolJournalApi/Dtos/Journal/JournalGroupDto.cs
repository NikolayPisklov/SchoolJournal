namespace SchoolJournalApi.Dtos.Journal
{
    public class JournalGroupDto
    {
        public int ClassId { get; set; }
        public string ClassTitle { get; set; } = string.Empty;
        public int ClassYear { get; set; }
        public List<JournalInListDto> JournalsOfClass { get; set; } = new List<JournalInListDto>();
    }
}

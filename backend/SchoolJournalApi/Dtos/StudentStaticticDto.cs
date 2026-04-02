namespace SchoolJournalApi.Dtos
{
    public class StudentStaticticDto
    {
        public List<DateOnly> DateLabels { get; set; } = new List<DateOnly>();
        public List<double> FactMarks { get; set; } = new List<double>();
        public List<double> AvgMarks { get; set; } = new List<double>();
    }
}

namespace SchoolJournalApi.Dto_s
{
    public class PagingResultDto<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int NumberOfPages { get; set; } = 1;
    }
}

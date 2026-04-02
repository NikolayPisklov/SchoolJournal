namespace SchoolJournalApi.Exceptions
{
    public class EfDbException : AppException
    {
        public EfDbException(string message) : base(message)
        {
        }
    }
}

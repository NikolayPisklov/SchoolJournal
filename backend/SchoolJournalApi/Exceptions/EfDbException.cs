namespace SchoolJournalApi.Exceptions
{
    public class EfDbException : AppException
    {
        public EfDbException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

namespace SchoolJournalApi.Exceptions
{
    public class EntityAddingException: AppException
    {
        public EntityAddingException(string message, Exception innerException) : base (message, innerException)
        { }
    }
}

namespace SchoolJournalApi.Exceptions
{
    public class EntityUpdateException : AppException
    {
        public EntityUpdateException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}

namespace SchoolJournalApi.Exceptions
{
    public class EntityInUseException : AppException
    {
        public EntityInUseException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}

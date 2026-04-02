namespace SchoolJournalApi.Exceptions
{
    public class EntityAlreadyExistsException : AppException
    {
        public EntityAlreadyExistsException(string message) : base(message)
        {
        }
    }
}

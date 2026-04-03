namespace SchoolJournalApi.Exceptions
{
    public class EntityHasBusinessLogicConflictException : AppException
    {
        public EntityHasBusinessLogicConflictException(string message) : base(message)
        {
        }
    }
}

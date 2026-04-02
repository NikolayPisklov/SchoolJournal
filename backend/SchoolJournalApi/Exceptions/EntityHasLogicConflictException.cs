namespace SchoolJournalApi.Exceptions
{
    public class EntityHasLogicConflictException : AppException
    {
        public EntityHasLogicConflictException(string message) : base(message)
        {
        }
    }
}

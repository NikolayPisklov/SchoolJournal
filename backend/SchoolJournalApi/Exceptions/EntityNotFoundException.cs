namespace SchoolJournalApi.Exceptions
{
    public class EntityNotFoundException : AppException
    {
        public EntityNotFoundException(string message) 
            : base(message) { }
    }
}

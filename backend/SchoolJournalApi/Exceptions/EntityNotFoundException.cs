namespace SchoolJournalApi.Exceptions
{
    public class EntityNotFoundException : AppException
    {
        public EntityNotFoundException(string entity) 
            : base($"{entity} with is not found."){ }
    }
}

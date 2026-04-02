namespace SchoolJournalApi.Exceptions
{
    public class EntityInUseException : AppException
    {
        public EntityInUseException(string entity, int id) 
            : base($"{entity} with id {id} is in use in other tables.") { }
    }
}

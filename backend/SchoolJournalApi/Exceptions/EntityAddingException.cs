namespace SchoolJournalApi.Exceptions
{
    public class EntityAddingException: AppException
    {
        public EntityAddingException(string entity, string desc = "") 
            : base($"Failed to add ${entity} to a database. {desc}") { }
    }
}

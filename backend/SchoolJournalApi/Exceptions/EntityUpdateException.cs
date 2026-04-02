namespace SchoolJournalApi.Exceptions
{
    public class EntityUpdateException : AppException
    {
        public EntityUpdateException(string entity, string desc = "") : base($"Failed to update {entity}. {desc}"){ }
    }
}

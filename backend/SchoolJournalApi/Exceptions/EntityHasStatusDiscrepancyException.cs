namespace SchoolJournalApi.Exceptions
{
    public class EntityHasStatusDiscrepancyException : AppException
    {
        public EntityHasStatusDiscrepancyException(int id, string desc) 
            : base($"User with Id: {id} is other status than needed for executed operation. {desc}"){ }
    }
}

namespace SchoolJournalApi.Exceptions
{
    public class EntityHasStatusDiscrepancyException : AppException
    {
        public EntityHasStatusDiscrepancyException(string message) 
            : base(message) { }
    }
}

namespace SchoolJournal.ViewModels
{
    public class SchoolDateTime
    {
        public static int GetCurrentYearId(SchoolJournalContext db) 
        {
            return db.SchoolYears.Where(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now).Select(s => s.Id).First();
        }
    }
}

namespace SchoolJournalApi.Services.AppServices
{
    public static class SchoolYearService
    {
        public static int GetCurrentSchoolYear()
        {
            return 2026;
            //var currentDate = DateTime.Now;
            //if(currentDate.Month >= 1 && currentDate.Month < 6) 
            //{
            //    return currentDate.Year - 1;
            //}
            //else
            //{
            //    return currentDate.Year;
            //}
        }
    }
}

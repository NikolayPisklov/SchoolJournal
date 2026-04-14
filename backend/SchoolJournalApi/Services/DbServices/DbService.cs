using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices
{
    public class DbService
    {
        protected SchoolJournalDbContext _db;

        public DbService(SchoolJournalDbContext db) 
        {
            _db = db;
        }
    }
}

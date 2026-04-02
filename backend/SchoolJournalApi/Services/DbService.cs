using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services
{
    public class DbService<T>
    {
        protected SchoolJournalDbContext _db;

        public DbService(SchoolJournalDbContext db) 
        {
            _db = db;
        }

        protected async Task<int> GetNumberOfItemsPagesAsync(IQueryable<T>? items, int pageSize)
        {
            if(items == null) 
            {
                return 0; 
            }
            double numberOfClasses = await items.CountAsync();
            double result = numberOfClasses / pageSize;
            return (int)Math.Ceiling(result);
        }
    }
}

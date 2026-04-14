using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.DbServices
{
    public class ClassDbService : DbService, IClassDbService
    {
        public ClassDbService (SchoolJournalDbContext db) : base (db)
        {
          
        }
        public void AddClass(Class newClass)
        {
            _db.Add(newClass);
        }
        public async Task<Class?> FindClassAsync(int classId) 
        {
            return await _db.Classes.FindAsync(classId);
        }
        public void DeleteClass(Class classEntity)
        {
             _db.Remove(classEntity);
        }
        public IQueryable<Class> GetClasses() 
        {
            return _db.Classes.AsNoTracking();
        }
    }
}

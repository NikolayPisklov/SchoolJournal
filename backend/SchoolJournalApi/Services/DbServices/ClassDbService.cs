using Microsoft.Data.SqlClient;
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
        public async Task<bool> IsThereStudentsInClass(int classId)
        {
            try
            {
                return await _db.StudentClasses.Where(x => x.ClassId == classId && x.IsActive).AnyAsync();
            }
            catch(SqlException ex)
            {
                throw new EfDbException("An error has occured while reading data from DB.", ex);
            }
        }
        public async Task<bool> IsThereJournalsForClass(int classId, int year)
        {
            try
            {
                return await _db.Journals.Where(x => x.ClassId == classId && x.Year == year).AnyAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occured while reading data from DB.", ex);
            }            
        }
    }
}

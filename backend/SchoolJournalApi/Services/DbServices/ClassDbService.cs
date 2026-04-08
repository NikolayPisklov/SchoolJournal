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
        public async Task AddClassAsync(Class newClass)
        {
            try
            {
                await _db.AddAsync(newClass);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityAddingException("An error has occurred while adding a Class entity!", ex);
            }
        }
        public async Task<Class?> FindClassAsync(int classId) 
        {
            return await _db.Classes.FindAsync(classId);
        }
        public async Task DeleteClassAsync(Class classEntity)
        {
            try
            {
                _db.Remove(classEntity);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new EntityInUseException($"Entity Class with Id: {classEntity.Id} is in use and can't be deleted!", ex);
            }
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new EntityUpdateException("An error has occurred while updating a Class entity!", ex);
            }
        }
        public IQueryable<Class> GetClasses() 
        {
            return _db.Classes.AsNoTracking();
        }
    }
}

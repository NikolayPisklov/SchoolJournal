using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class LessonDbService : DbService, ILessonDbService
    {
        public LessonDbService(SchoolJournalDbContext db) : base(db) { }

        public async Task AddLessonAsync(Lesson lesson) 
        {
            try
            {
                _db.Lessons.Add(lesson);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException ex) 
            {
                throw new EntityAddingException("An error has occur while adding lesson to DB!", ex);
            }
        }
        public async Task<Lesson?> FindLessonAsync(int lessonId) 
        {
            try
            {
                return await _db.Lessons.FindAsync(lessonId);
            }
            catch(DbException ex) 
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }
        }
        public async Task DeleteLessonAsync(Lesson lesson) 
        {
            try
            {
                _db.Lessons.Remove(lesson);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityInUseException($"Lesson with Id: {lesson.Id} is in use and can't be deleted", ex);
            }
        }
        public IQueryable<Lesson> GetLessonsForJournal(int journalId, int month, int journalYear) 
        {          
            var start = new DateOnly(journalYear, month, 1);
            var end = start.AddMonths(1);
            return _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.LessonDate >= start && l.LessonDate < end 
                && l.IsDeleted == false)
                .OrderBy(l => l.LessonDate);
        }
        public async Task SaveChangesAsync() 
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EfDbException("An error has occur while updating data!", ex);
            }
        }
    }
}

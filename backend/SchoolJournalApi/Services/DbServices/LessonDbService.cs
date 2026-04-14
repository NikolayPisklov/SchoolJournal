using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class LessonDbService : DbService, ILessonDbService
    {
        public LessonDbService(SchoolJournalDbContext db) : base(db) { }

        public void AddLesson(Lesson lesson) 
        {
            _db.Lessons.Add(lesson);
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
        public void DeleteLesson(Lesson lesson) 
        {
            _db.Lessons.Remove(lesson);
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

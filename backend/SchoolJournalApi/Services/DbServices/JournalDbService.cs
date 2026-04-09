using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Models;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class JournalDbService : DbService, IJournalDbService
    {
        public JournalDbService(SchoolJournalDbContext db) : base(db)
        {
        }
        public async Task<bool> IsThereSameJournal(int classId, int teacherSubjectId, int year) 
        {
            try
            {
                return await _db.Journals.AnyAsync(j => j.ClassId == classId
                    && j.TeacherSubjectId == teacherSubjectId && j.Year == year);
            }
            catch (DbException ex) 
            {
                throw new EntityAddingException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task AddJournalAsync(Journal journal)
        {
            try
            {
                _db.Add(journal);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityAddingException("An error has occurred while adding a Journal entity!", ex);
            }
        }
        public async Task<Journal?> FindJournalAsync(int journalId)
        {
            try
            {
                var journal = await _db.Journals.FindAsync(journalId);
                return journal;
            }
            catch (DbException ex)
            {
                throw new EntityAddingException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<Journal?> FindJournalWithIncludes(int journalId) 
        {
            try
            {
                var journal = await _db.Journals.Include(j => j.Class).Include(j => j.TeacherSubject)
                    .ThenInclude(ts => ts.Subject).FirstOrDefaultAsync(j => j.Id == journalId);
                return journal;
            }
            catch (DbException ex)
            {
                throw new EntityAddingException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<StudentClass?> FindStudentClassAsync(int studentId) 
        {
            try
            {
                var studentClass = await _db.StudentClasses.FirstOrDefaultAsync(sc => sc.IsActive 
                    && sc.UserId == studentId);
                return studentClass;
            }
            catch (DbException ex)
            {
                throw new EntityAddingException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task DeleteJournalAsync(Journal journal)
        {
            try
            {
                _db.Remove(journal);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new EntityInUseException($"Entity Journal with Id: {journal.Id} is in use and can't be deleted!", ex);
            }
        }
        public IQueryable<StudentClass> GetStudentsForJournal(int journalId) 
        {
            return _db.StudentClasses.AsNoTracking()
                .Where(sc => sc.ClassId == journalId);            
        }
        public IQueryable<Lesson> GetLessonsForJournal(int journalId, int journalYear) 
        {
            var start = new DateOnly(journalYear, 9, 1);
            var end = start.AddMonths(1);
            return _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.LessonDate >= start
                    && l.LessonDate < end && l.IsDeleted == false)
                .OrderBy(l => l.LessonDate);
        }
        public IQueryable<Progress> GetProgressesForJournal(int journalId) 
        {
            return _db.Progresses.AsNoTracking()
            .Where(prog => prog.Lesson.JournalId == journalId
                && prog.IsUpdated == false);
        }
        public IQueryable<Journal> GetJournalsForClass(int classId) 
        {
            return _db.Journals.AsNoTracking()
                .Where(j => j.ClassId == classId);
        }
        public IQueryable<Journal> GetJournalsForTeacher(int teacherId) 
        {
            return _db.Journals.AsNoTracking()
                .Where(j => j.TeacherSubject.Teacher.Id == teacherId);
        }
       
    }
}

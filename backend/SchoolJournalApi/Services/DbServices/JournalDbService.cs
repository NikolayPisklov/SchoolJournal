using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Models;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Dtos.Journal;
using SchoolJournalApi.Dtos.Lesson;
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
        //======================================================
        public async Task<JournalDetailsDto> GetJournalDetailsForStudent(int journalId, int studentId)
        {
            var journalDto = new JournalDetailsDto();
            var journal = await _db.Journals.FindAsync(journalId);
            if (journal is null)
                throw new EntityNotFoundException("Journal");
            var lessons = await _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.IsDeleted == false)
                .OrderBy(l => l.LessonDate)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            var progresses = await _db.Progresses.AsNoTracking()
                .Where(prog => prog.Lesson.JournalId == journalId
                    && prog.IsUpdated == false && prog.UserId == studentId)
                .Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance!.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();            
            journalDto.Lessons = lessons;
            journalDto.Progresses = progresses;
            journalDto.JournalYear = journal.Year;
            return journalDto;
        }

        public async Task<List<JournalInListDto>> GetJournalsForClassAsync(int classId)
        {
            var journals = await _db.Journals.AsNoTracking()
                .Where(j => j.ClassId == classId)
                .Select(j => new JournalInListDto()
                {
                    Id = j.Id,
                    ClassId = j.ClassId,
                    TeacherSubjectId = j.TeacherSubjectId,
                    SubjectTitle = j.TeacherSubject!.Subject!.Title,
                    TeacherFirstName = j.TeacherSubject!.Teacher!.FirstName,
                    TeacherLastName = j.TeacherSubject.Teacher.LastName,
                    TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                    ClassTitle = j.Class!.Title,
                    ClassYear = j.Class.Year,
                    JournalYear = j.Year
                }).ToListAsync();
            return journals;
        }

        public async Task<List<JournalInListDto>> GetJournalsForStudent(int studentId)
        {
            var studentClass = await _db.StudentClasses.AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == studentId && s.IsActive);
            if(studentClass is null)
            {
                throw new EntityNotFoundException($"Active journal for student with Id: {studentId} is not found");
            }
            var journals = await _db.Journals.AsNoTracking()
                .Where(j => j.ClassId == studentClass.ClassId && j.Year == DateTime.Now.Year)
                .Select(j => new JournalInListDto{
                    Id = j.Id,
                    ClassId = j.ClassId,
                    TeacherSubjectId = j.TeacherSubjectId,
                    SubjectTitle = j.TeacherSubject.Subject.Title,
                    TeacherFirstName = j.TeacherSubject.Teacher.FirstName,
                    TeacherLastName = j.TeacherSubject.Teacher.LastName,
                    TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                    ClassTitle = j.Class.Title,
                    ClassYear = j.Class.Year,
                    JournalYear = j.Year
                }).ToListAsync();
            return journals;
        }

        public async Task<List<JournalGroupDto>> GetJournalsForTeacherAsync(int teacherId)
        {
            var journals = await _db.Journals.AsNoTracking()
               .Where(j => j.TeacherSubject!.Teacher!.Id == teacherId)
               .OrderBy(j => j.Class!.Year)
               .ThenBy(j => j.Class!.Title)
               .GroupBy(j => new
               {
                   j.ClassId,
                   j.Class!.Title,
                   j.Class.Year
               })
               .Select(g => new JournalGroupDto
               {
                   ClassId = g.Key.ClassId,
                   ClassTitle = g.Key.Title,
                   ClassYear = g.Key.Year,

                   JournalsOfClass = g.Select(j => new JournalInListDto
                   {
                       Id = j.Id,
                       SubjectTitle = j.TeacherSubject.Subject.Title,
                       TeacherFirstName = j.TeacherSubject.Teacher.FirstName,
                       TeacherLastName = j.TeacherSubject.Teacher.LastName,
                       TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                       JournalYear = j.Year
                   }).ToList()
               }).ToListAsync();
            return journals;
        }

        public async Task<JournalTitleDto> GetJournalTitleAsync(int journalId)
        {
            var dto = await _db.Journals.Where(j => j.Id == journalId)
                .Select(j => new JournalTitleDto 
                {
                    ClassTitle = j.Class!.Title,
                    ClassYear = j.Class.Year,
                    SubjectTitle = j.TeacherSubject.Subject.Title
                }).FirstAsync();
            return dto;
        }
    }
}
